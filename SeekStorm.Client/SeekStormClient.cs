using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeekStorm.Client;

public sealed class SeekStormClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public string BaseUrl { get; }

    public string? ApiKeyBase64 { get; }

    public SeekStormClient(string baseUrl, string? apiKeyBase64 = null, TimeSpan? timeout = null, HttpClient? httpClient = null)
    {
        BaseUrl = baseUrl.TrimEnd('/');
        ApiKeyBase64 = apiKeyBase64;
        _ownsHttpClient = httpClient is null;
        _httpClient = httpClient ?? new HttpClient();

        if (timeout is not null)
        {
            _httpClient.Timeout = timeout.Value;
        }
        else if (_ownsHttpClient)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    public LiveResponse Live(string? baseUrl = null)
        => LiveAsync(baseUrl).GetAwaiter().GetResult();

    public async Task<LiveResponse> LiveAsync(string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, "/api/v1/live", null, null, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return new LiveResponse { Message = text };
    }

    public ApiKeyResponse CreateApikey(string masterApikey, ApikeyQuotaObject apiKeyQuotaObject, string? baseUrl = null)
        => CreateApikeyAsync(masterApikey, apiKeyQuotaObject, baseUrl).GetAwaiter().GetResult();

    public async Task<ApiKeyResponse> CreateApikeyAsync(string masterApikey, ApikeyQuotaObject apiKeyQuotaObject, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, "/api/v1/apikey", apiKeyQuotaObject, masterApikey, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return new ApiKeyResponse { ApiKeyBase64 = text };
    }

    public RemainingApiKeysResponse DeleteApikey(string apikeyBase64, string masterApikeyBase64, string? baseUrl = null)
        => DeleteApikeyAsync(apikeyBase64, masterApikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<RemainingApiKeysResponse> DeleteApikeyAsync(string apikeyBase64, string masterApikeyBase64, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteApikeyRequest { ApikeyBase64 = apikeyBase64 };
        using var request = CreateRequest(HttpMethod.Delete, "/api/v1/apikey", deleteRequest, masterApikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var value = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new RemainingApiKeysResponse { RemainingApiKeys = value };
    }

    public ApikeyInfoResponse GetApikeyInfo(string? apikeyBase64 = null, string? baseUrl = null)
        => GetApikeyInfoAsync(apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<ApikeyInfoResponse> GetApikeyInfoAsync(string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, "/api/v1/apikey", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        return ParseApikeyInfoPayload(doc.RootElement);
    }

    public CreateIndexResponse CreateIndex(CreateIndexRequest request, string? apikeyBase64 = null, string? baseUrl = null)
        => CreateIndexAsync(request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<CreateIndexResponse> CreateIndexAsync(CreateIndexRequest request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, "/api/v1/index", request, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var indexId = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new CreateIndexResponse { IndexId = indexId };
    }

    public RemainingIndicesResponse DeleteIndex(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null)
        => DeleteIndexAsync(indexId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<RemainingIndicesResponse> DeleteIndexAsync(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, $"/api/v1/index/{indexId}", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var remaining = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new RemainingIndicesResponse { RemainingIndices = remaining };
    }

    public IndexedDocumentCountResponse ClearIndex(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null)
        => ClearIndexAsync(indexId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> ClearIndexAsync(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, $"/api/v1/index/{indexId}/doc", null, apikeyBase64, baseUrl);
        request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("clear"));
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse CommitIndex(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null)
        => CommitIndexAsync(indexId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> CommitIndexAsync(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Patch, $"/api/v1/index/{indexId}", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexResponseObject GetIndexInfo(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null)
        => GetIndexInfoAsync(indexId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexResponseObject> GetIndexInfoAsync(ulong indexId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, $"/api/v1/index/{indexId}", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return await DeserializeAsync<IndexResponseObject>(response, cancellationToken).ConfigureAwait(false);
    }

    public IndexedDocumentCountResponse IndexDocument(ulong indexId, Dictionary<string, object?> document, string? apikeyBase64 = null, string? baseUrl = null)
        => IndexDocumentAsync(indexId, document, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> IndexDocumentAsync(ulong indexId, Dictionary<string, object?> document, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, $"/api/v1/index/{indexId}/doc", document, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse IndexDocuments(ulong indexId, IEnumerable<Dictionary<string, object?>> documents, string? apikeyBase64 = null, string? baseUrl = null)
        => IndexDocumentsAsync(indexId, documents, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> IndexDocumentsAsync(ulong indexId, IEnumerable<Dictionary<string, object?>> documents, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, $"/api/v1/index/{indexId}/doc", documents.ToList(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse IndexPdf(ulong indexId, string filePath, long fileDate, byte[] document, string? apikeyBase64 = null, string? baseUrl = null)
        => IndexPdfAsync(indexId, filePath, fileDate, document, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> IndexPdfAsync(ulong indexId, string filePath, long fileDate, byte[] document, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, $"/api/v1/index/{indexId}/file", null, apikeyBase64, baseUrl);
        request.Headers.Add("file", filePath);
        request.Headers.Add("date", fileDate.ToString());
        request.Content = new ByteArrayContent(document);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public PdfResponse GetPdf(ulong indexId, ulong docId, string? apikeyBase64 = null, string? baseUrl = null)
        => GetPdfAsync(indexId, docId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<PdfResponse> GetPdfAsync(ulong indexId, ulong docId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, $"/api/v1/index/{indexId}/file/{docId}", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        return new PdfResponse { Content = content };
    }

    public DocumentResponse GetDocument(ulong indexId, ulong docId, GetDocumentRequest request, string? apikeyBase64 = null, string? baseUrl = null)
        => GetDocumentAsync(indexId, docId, request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<DocumentResponse> GetDocumentAsync(ulong indexId, ulong docId, GetDocumentRequest request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, $"/api/v1/index/{indexId}/doc/{docId}", request, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var body = await DeserializeAsync<Dictionary<string, JsonElement>>(response, cancellationToken).ConfigureAwait(false);
        return new DocumentResponse { Document = body };
    }

    public IndexedDocumentCountResponse UpdateDocument(ulong indexId, UpdateDocumentRequest request, string? apikeyBase64 = null, string? baseUrl = null)
        => UpdateDocumentAsync(indexId, request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> UpdateDocumentAsync(ulong indexId, UpdateDocumentRequest request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Patch, $"/api/v1/index/{indexId}/doc", request.ToPayload(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse UpdateDocuments(ulong indexId, UpdateDocumentsRequest request, string? apikeyBase64 = null, string? baseUrl = null)
        => UpdateDocumentsAsync(indexId, request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> UpdateDocumentsAsync(ulong indexId, UpdateDocumentsRequest request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Patch, $"/api/v1/index/{indexId}/doc", request.ToPayload(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse DeleteDocumentByDocid(ulong indexId, ulong docId, string? apikeyBase64 = null, string? baseUrl = null)
        => DeleteDocumentByDocidAsync(indexId, docId, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> DeleteDocumentByDocidAsync(ulong indexId, ulong docId, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, $"/api/v1/index/{indexId}/doc/{docId}", null, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse DeleteDocumentsByDocid(ulong indexId, IEnumerable<ulong> docIdVec, string? apikeyBase64 = null, string? baseUrl = null)
        => DeleteDocumentsByDocidAsync(indexId, docIdVec, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> DeleteDocumentsByDocidAsync(ulong indexId, IEnumerable<ulong> docIdVec, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, $"/api/v1/index/{indexId}/doc", docIdVec.ToList(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IndexedDocumentCountResponse DeleteDocumentsByQuery(ulong indexId, SearchRequestObject query, string? apikeyBase64 = null, string? baseUrl = null)
        => DeleteDocumentsByQueryAsync(indexId, query, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IndexedDocumentCountResponse> DeleteDocumentsByQueryAsync(ulong indexId, SearchRequestObject query, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, $"/api/v1/index/{indexId}/doc", query.ToPayload(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var count = await ParseUlongBodyAsync(response, cancellationToken).ConfigureAwait(false);
        return new IndexedDocumentCountResponse { IndexedDocumentCount = count };
    }

    public IteratorResult DocumentIterator(ulong indexId, GetIteratorRequest request, string? apikeyBase64 = null, string? baseUrl = null)
        => DocumentIteratorAsync(indexId, request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<IteratorResult> DocumentIteratorAsync(ulong indexId, GetIteratorRequest request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, $"/api/v1/index/{indexId}/iterator", request, apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return await DeserializeAsync<IteratorResult>(response, cancellationToken).ConfigureAwait(false);
    }

    public SearchResultObject QueryIndex(ulong indexId, SearchRequestObject request, string? apikeyBase64 = null, string? baseUrl = null)
        => QueryIndexAsync(indexId, request, apikeyBase64, baseUrl).GetAwaiter().GetResult();

    public async Task<SearchResultObject> QueryIndexAsync(ulong indexId, SearchRequestObject request, string? apikeyBase64 = null, string? baseUrl = null, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, $"/api/v1/index/{indexId}/query", request.ToPayload(), apikeyBase64, baseUrl);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return await DeserializeAsync<SearchResultObject>(response, cancellationToken).ConfigureAwait(false);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, object? payload, string? apikey, string? baseUrl)
    {
        var request = new HttpRequestMessage(method, BuildUrl(path, baseUrl));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var key = apikey ?? ApiKeyBase64;
        if (!string.IsNullOrWhiteSpace(key))
        {
            request.Headers.Add("apikey", key);
        }

        if (payload is not null)
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private string BuildUrl(string path, string? baseUrl)
    {
        var resolvedBase = (baseUrl ?? BaseUrl).TrimEnd('/');
        return $"{resolvedBase}{path}";
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        throw new SeekStormApiException((int)response.StatusCode, body);
    }

    private async Task<ulong> ParseUlongBodyAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        var body = (await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).Trim();
        if (ulong.TryParse(body, out var value))
        {
            return value;
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Number && root.TryGetUInt64(out var intValue))
            {
                return intValue;
            }

            if (root.ValueKind == JsonValueKind.String && ulong.TryParse(root.GetString(), out var intFromString))
            {
                return intFromString;
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in new[] { "Ok", "ok", "value", "result" })
                {
                    if (!root.TryGetProperty(key, out var property))
                    {
                        continue;
                    }

                    if (property.ValueKind == JsonValueKind.Number && property.TryGetUInt64(out var numberValue))
                    {
                        return numberValue;
                    }

                    if (property.ValueKind == JsonValueKind.String && ulong.TryParse(property.GetString(), out var stringValue))
                    {
                        return stringValue;
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Fall through to explicit exception below.
        }

        throw new SeekStormApiException((int)response.StatusCode, $"Expected unsigned integer response, got: {body}");
    }

    private async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var parsed = JsonSerializer.Deserialize<T>(text, _jsonOptions);
        if (parsed is null)
        {
            throw new SeekStormApiException((int)response.StatusCode, $"Failed to deserialize response into {typeof(T).Name}: {text}");
        }

        return parsed;
    }

    private static ApikeyInfoResponse ParseApikeyInfoPayload(JsonElement payload)
    {
        if (payload.ValueKind == JsonValueKind.Array)
        {
            return new ApikeyInfoResponse
            {
                Indices = DeserializeIndexList(payload),
            };
        }

        if (payload.ValueKind == JsonValueKind.Object)
        {
            foreach (var key in new[] { "indices", "result", "Ok", "ok", "data" })
            {
                if (payload.TryGetProperty(key, out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    return new ApikeyInfoResponse
                    {
                        Indices = DeserializeIndexList(listElement),
                    };
                }
            }

            if (payload.TryGetProperty("id", out _) && payload.TryGetProperty("name", out _))
            {
                var index = payload.Deserialize<IndexResponseObject>();
                return new ApikeyInfoResponse
                {
                    Indices = index is null ? new List<IndexResponseObject>() : new List<IndexResponseObject> { index },
                };
            }
        }

        return new ApikeyInfoResponse();
    }

    private static List<IndexResponseObject> DeserializeIndexList(JsonElement element)
    {
        var list = new List<IndexResponseObject>();
        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var parsed = item.Deserialize<IndexResponseObject>();
            if (parsed is not null)
            {
                list.Add(parsed);
            }
        }

        return list;
    }
}
