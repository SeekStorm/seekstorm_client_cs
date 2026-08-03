using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeekStorm.Client;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LexicalSimilarity
{
    Bm25f,
    Bm25fProximity,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TokenizerType
{
    AsciiAlphabetic,
    UnicodeAlphanumeric,
    UnicodeAlphanumericFolded,
    UnicodeAlphanumericZH,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StemmerType
{
    None,
    English,
    German,
    Spanish,
    French,
    Italian,
    Portuguese,
    Russian,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StopwordType
{
    None,
    English,
    German,
    Spanish,
    French,
    Italian,
    Portuguese,
    Russian,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FrequentwordType
{
    None,
    English,
    German,
    Spanish,
    French,
    Italian,
    Portuguese,
    Russian,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentCompression
{
    None,
    Lz4,
    Snappy,
    Zstd,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultType
{
    Count,
    Topk,
    TopkCount,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QueryType
{
    Union,
    Intersection,
    Phrase,
    Not,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QueryRewriting
{
    SearchOnly,
    Auto,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchMode
{
    Lexical,
    Vector,
    Hybrid,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FieldType
{
    U8,
    U16,
    U32,
    U64,
    I8,
    I16,
    I32,
    I64,
    Timestamp,
    F32,
    F64,
    Bool,
    String16,
    String32,
    StringSet16,
    StringSet32,
    Point,
    Text,
    Json,
    Binary,
}

public sealed class LiveResponse
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}

public sealed class ApiKeyResponse
{
    [JsonPropertyName("api_key_base64")]
    public string ApiKeyBase64 { get; init; } = string.Empty;
}

public sealed class RemainingApiKeysResponse
{
    [JsonPropertyName("remaining_api_keys")]
    public ulong RemainingApiKeys { get; init; }
}

public sealed class ApikeyQuotaObject
{
    [JsonPropertyName("indices_max")]
    public int IndicesMax { get; init; }

    [JsonPropertyName("indices_size_max")]
    public int IndicesSizeMax { get; init; }

    [JsonPropertyName("documents_max")]
    public int DocumentsMax { get; init; }

    [JsonPropertyName("operations_max")]
    public int OperationsMax { get; init; }

    [JsonPropertyName("rate_limit")]
    public int? RateLimit { get; init; }

    [JsonPropertyName("demo")]
    public bool Demo { get; init; }
}

public sealed class DeleteApikeyRequest
{
    [JsonPropertyName("apikey_base64")]
    public string ApikeyBase64 { get; init; } = string.Empty;
}

public sealed class SchemaField
{
    [JsonPropertyName("field")]
    public string Field { get; init; } = string.Empty;

    [JsonPropertyName("field_type")]
    public FieldType FieldType { get; init; }

    [JsonPropertyName("store")]
    public bool Store { get; init; }

    [JsonPropertyName("index_lexical")]
    public bool IndexLexical { get; init; }

    [JsonPropertyName("index_vector")]
    public bool? IndexVector { get; init; }

    [JsonPropertyName("facet")]
    public bool? Facet { get; init; }

    [JsonPropertyName("boost")]
    public float? Boost { get; init; }

    [JsonPropertyName("longest")]
    public bool? Longest { get; init; }

    [JsonPropertyName("dictionary_source")]
    public bool? DictionarySource { get; init; }

    [JsonPropertyName("completion_source")]
    public bool? CompletionSource { get; init; }
}

public sealed class Synonym
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class SpellingCorrectionConfig
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class QueryCompletionConfig
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class ClusteringConfig
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class InferenceConfig
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class CreateIndexRequest
{
    [JsonPropertyName("index_name")]
    public string IndexName { get; init; } = string.Empty;

    [JsonPropertyName("schema")]
    public List<SchemaField> Schema { get; init; } = new();

    [JsonPropertyName("similarity")]
    public LexicalSimilarity? Similarity { get; init; }

    [JsonPropertyName("tokenizer")]
    public TokenizerType? Tokenizer { get; init; }

    [JsonPropertyName("stemmer")]
    public StemmerType? Stemmer { get; init; }

    [JsonPropertyName("stop_words")]
    public StopwordType? StopWords { get; init; }

    [JsonPropertyName("frequent_words")]
    public FrequentwordType? FrequentWords { get; init; }

    [JsonPropertyName("ngram_indexing")]
    public int? NgramIndexing { get; init; }

    [JsonPropertyName("document_compression")]
    public DocumentCompression? DocumentCompression { get; init; }

    [JsonPropertyName("synonyms")]
    public List<Synonym>? Synonyms { get; init; }

    [JsonPropertyName("spelling_correction")]
    public SpellingCorrectionConfig? SpellingCorrection { get; init; }

    [JsonPropertyName("query_completion")]
    public QueryCompletionConfig? QueryCompletion { get; init; }

    [JsonPropertyName("clustering")]
    public ClusteringConfig? Clustering { get; init; }

    [JsonPropertyName("inference")]
    public InferenceConfig? Inference { get; init; }
}

public sealed class GetIteratorRequest
{
    [JsonPropertyName("document_id")]
    public ulong? DocumentId { get; init; }

    [JsonPropertyName("skip")]
    public int Skip { get; init; }

    [JsonPropertyName("take")]
    public int Take { get; init; } = 1;

    [JsonPropertyName("include_deleted")]
    public bool IncludeDeleted { get; init; }

    [JsonPropertyName("include_document")]
    public bool IncludeDocument { get; init; }

    [JsonPropertyName("fields")]
    public List<string> Fields { get; init; } = new();
}

public sealed class Highlight
{
    [JsonPropertyName("field")]
    public string Field { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("fragment_number")]
    public int FragmentNumber { get; init; }

    [JsonPropertyName("fragment_size")]
    public int FragmentSize { get; init; }

    [JsonPropertyName("highlight_markup")]
    public bool HighlightMarkup { get; init; }

    [JsonPropertyName("pre_tags")]
    public string? PreTags { get; init; }

    [JsonPropertyName("post_tags")]
    public string? PostTags { get; init; }
}

public sealed class DistanceField
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class QueryFacet
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class FacetFilterItem
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class ResultSortItem
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Properties { get; init; }
}

public sealed class GetDocumentRequest
{
    [JsonPropertyName("query_terms")]
    public List<string> QueryTerms { get; init; } = new();

    [JsonPropertyName("highlights")]
    public List<Highlight> Highlights { get; init; } = new();

    [JsonPropertyName("fields")]
    public List<string> Fields { get; init; } = new();

    [JsonPropertyName("distance_fields")]
    public List<DistanceField> DistanceFields { get; init; } = new();
}

public sealed class SearchRequestObject
{
    [JsonIgnore]
    public string QueryString { get; init; } = string.Empty;

    [JsonPropertyName("query_vector")]
    public object? QueryVector { get; init; }

    [JsonPropertyName("enable_empty_query")]
    public bool EnableEmptyQuery { get; init; }

    [JsonPropertyName("offset")]
    public int Offset { get; init; }

    [JsonPropertyName("length")]
    public int Length { get; init; } = 10;

    [JsonPropertyName("result_type")]
    public ResultType ResultType { get; init; } = ResultType.TopkCount;

    [JsonPropertyName("realtime")]
    public bool Realtime { get; init; }

    [JsonPropertyName("highlights")]
    public List<Highlight> Highlights { get; init; } = new();

    [JsonPropertyName("field_filter")]
    public List<string> FieldFilter { get; init; } = new();

    [JsonPropertyName("fields")]
    public List<string> Fields { get; init; } = new();

    [JsonPropertyName("distance_fields")]
    public List<DistanceField> DistanceFields { get; init; } = new();

    [JsonPropertyName("query_facets")]
    public List<QueryFacet> QueryFacets { get; init; } = new();

    [JsonPropertyName("facet_filter")]
    public List<FacetFilterItem> FacetFilter { get; init; } = new();

    [JsonPropertyName("result_sort")]
    public List<ResultSortItem> ResultSort { get; init; } = new();

    [JsonPropertyName("query_type_default")]
    public QueryType QueryTypeDefault { get; init; } = QueryType.Intersection;

    [JsonPropertyName("query_rewriting")]
    public QueryRewriting QueryRewriting { get; init; } = QueryRewriting.SearchOnly;

    [JsonPropertyName("search_mode")]
    public SearchMode SearchMode { get; init; } = SearchMode.Lexical;

    public SearchRequestPayload ToPayload()
    {
        return new SearchRequestPayload
        {
            Query = QueryString,
            QueryVector = QueryVector,
            EnableEmptyQuery = EnableEmptyQuery,
            Offset = Offset,
            Length = Length,
            ResultType = ResultType,
            Realtime = Realtime,
            Highlights = Highlights,
            FieldFilter = FieldFilter,
            Fields = Fields,
            DistanceFields = DistanceFields,
            QueryFacets = QueryFacets,
            FacetFilter = FacetFilter,
            ResultSort = ResultSort,
            QueryTypeDefault = QueryTypeDefault,
            QueryRewriting = QueryRewriting,
            SearchMode = SearchMode,
        };
    }
}

public sealed class SearchRequestPayload
{
    [JsonPropertyName("query")]
    public string Query { get; init; } = string.Empty;

    [JsonPropertyName("query_vector")]
    public object? QueryVector { get; init; }

    [JsonPropertyName("enable_empty_query")]
    public bool EnableEmptyQuery { get; init; }

    [JsonPropertyName("offset")]
    public int Offset { get; init; }

    [JsonPropertyName("length")]
    public int Length { get; init; }

    [JsonPropertyName("result_type")]
    public ResultType ResultType { get; init; }

    [JsonPropertyName("realtime")]
    public bool Realtime { get; init; }

    [JsonPropertyName("highlights")]
    public List<Highlight> Highlights { get; init; } = new();

    [JsonPropertyName("field_filter")]
    public List<string> FieldFilter { get; init; } = new();

    [JsonPropertyName("fields")]
    public List<string> Fields { get; init; } = new();

    [JsonPropertyName("distance_fields")]
    public List<DistanceField> DistanceFields { get; init; } = new();

    [JsonPropertyName("query_facets")]
    public List<QueryFacet> QueryFacets { get; init; } = new();

    [JsonPropertyName("facet_filter")]
    public List<FacetFilterItem> FacetFilter { get; init; } = new();

    [JsonPropertyName("result_sort")]
    public List<ResultSortItem> ResultSort { get; init; } = new();

    [JsonPropertyName("query_type_default")]
    public QueryType QueryTypeDefault { get; init; }

    [JsonPropertyName("query_rewriting")]
    public QueryRewriting QueryRewriting { get; init; }

    [JsonPropertyName("search_mode")]
    public SearchMode SearchMode { get; init; }
}

public sealed class UpdateDocumentRequest
{
    [JsonPropertyName("doc_id")]
    public ulong DocId { get; init; }

    [JsonPropertyName("document")]
    public Dictionary<string, object?> Document { get; init; } = new();

    public object[] ToPayload() => new object[] { DocId, Document };
}

public sealed class UpdateDocumentsRequest
{
    [JsonPropertyName("items")]
    public List<UpdateDocumentRequest> Items { get; init; } = new();

    public List<object[]> ToPayload() => Items.Select(i => i.ToPayload()).ToList();
}

public sealed class CreateIndexResponse
{
    [JsonPropertyName("index_id")]
    public ulong IndexId { get; init; }
}

public sealed class RemainingIndicesResponse
{
    [JsonPropertyName("remaining_indices")]
    public ulong RemainingIndices { get; init; }
}

public sealed class IndexedDocumentCountResponse
{
    [JsonPropertyName("indexed_document_count")]
    public ulong IndexedDocumentCount { get; init; }
}

public sealed class IndexResponseObject
{
    [JsonPropertyName("id")]
    public ulong Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("schema")]
    public Dictionary<string, Dictionary<string, JsonElement>> Schema { get; init; } = new();

    [JsonPropertyName("indexed_doc_count")]
    public ulong IndexedDocCount { get; init; }

    [JsonPropertyName("committed_doc_count")]
    public ulong CommittedDocCount { get; init; }

    [JsonPropertyName("operations_count")]
    public ulong OperationsCount { get; init; }

    [JsonPropertyName("query_count")]
    public ulong QueryCount { get; init; }

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [JsonPropertyName("facets_minmax")]
    public Dictionary<string, Dictionary<string, JsonElement>> FacetsMinmax { get; init; } = new();
}

public sealed class ApikeyInfoResponse
{
    [JsonPropertyName("indices")]
    public List<IndexResponseObject> Indices { get; init; } = new();
}

public sealed class IteratorResultItem
{
    [JsonPropertyName("doc_id")]
    public ulong DocId { get; init; }

    [JsonPropertyName("doc")]
    public Dictionary<string, JsonElement>? Doc { get; init; }
}

public sealed class IteratorResult
{
    [JsonPropertyName("skip")]
    public int Skip { get; init; }

    [JsonPropertyName("results")]
    public List<IteratorResultItem> Results { get; init; } = new();
}

public sealed class DocumentResponse
{
    [JsonPropertyName("document")]
    public Dictionary<string, JsonElement> Document { get; init; } = new();
}

public sealed class PdfResponse
{
    [JsonPropertyName("content")]
    public byte[] Content { get; init; } = Array.Empty<byte>();
}

public sealed class SearchResultObject
{
    [JsonPropertyName("time")]
    public ulong Time { get; init; }

    [JsonPropertyName("original_query")]
    public string OriginalQuery { get; init; } = string.Empty;

    [JsonPropertyName("query")]
    public string Query { get; init; } = string.Empty;

    [JsonPropertyName("offset")]
    public int Offset { get; init; }

    [JsonPropertyName("length")]
    public int Length { get; init; }

    [JsonPropertyName("count")]
    public ulong Count { get; init; }

    [JsonPropertyName("count_total")]
    public ulong CountTotal { get; init; }

    [JsonPropertyName("query_terms")]
    public List<string> QueryTerms { get; init; } = new();

    [JsonPropertyName("results")]
    public List<Dictionary<string, JsonElement>> Results { get; init; } = new();

    [JsonPropertyName("facets")]
    public Dictionary<string, JsonElement> Facets { get; init; } = new();

    [JsonPropertyName("suggestions")]
    public List<string> Suggestions { get; init; } = new();
}
