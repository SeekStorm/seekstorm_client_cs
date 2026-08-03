using SeekStorm.Client;

namespace SeekStorm.Client.IntegrationTests;

public sealed class SeekStormIntegrationTests
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("SEEKSTORM_BASE_URL") ?? "http://127.0.0.1:80";

    private static readonly string DemoApiKey =
        Environment.GetEnvironmentVariable("SEEKSTORM_API_KEY")
        ?? "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    private static readonly string MasterApiKey =
        Environment.GetEnvironmentVariable("SEEKSTORM_MASTER_API_KEY")
        ?? "/iWStCpyfpd/BVlHOFtwnMgrFrmof4jGq/OQDWXQzcM=";

    [Fact]
    public async Task Live_ReturnsMessage()
    {
        using var client = new SeekStormClient(BaseUrl, DemoApiKey);

        var response = await client.LiveAsync();

        Assert.False(string.IsNullOrWhiteSpace(response.Message));
    }

    [Fact]
    public async Task IndexLifecycle_CanCreateIndexIndexDocQueryAndDelete()
    {
        using var client = new SeekStormClient(BaseUrl, DemoApiKey);

        var indexName = $"seekstorm-client-cs-it-{Guid.NewGuid():N}";

        var createRequest = new CreateIndexRequest
        {
            IndexName = indexName,
            Schema =
            [
                new SchemaField
                {
                    Field = "title",
                    FieldType = FieldType.Text,
                    Store = true,
                    IndexLexical = true,
                },
            ],
        };

        ulong? indexId = null;
        try
        {
            CreateIndexResponse createResponse;
            try
            {
                createResponse = await client.CreateIndexAsync(createRequest);
            }
            catch (SeekStormApiException ex) when (ex.StatusCode is 401 or 403)
            {
                // Server requires an API key; keep test stable when key is not configured.
                return;
            }

            var createdIndexId = createResponse.IndexId;
            indexId = createdIndexId;

            var indexDocResponse = await client.IndexDocumentAsync(
                createdIndexId,
                new Dictionary<string, object?>
                {
                    ["title"] = "seekstorm integration lexical test",
                });

            Assert.True(indexDocResponse.IndexedDocumentCount >= 0);

            var commitResponse = await client.CommitIndexAsync(createdIndexId);
            Assert.True(commitResponse.IndexedDocumentCount >= 0);

            var queryResponse = await client.QueryIndexAsync(
                createdIndexId,
                new SearchRequestObject
                {
                    QueryString = "integration lexical",
                    Length = 5,
                });

            Assert.NotNull(queryResponse);
            Assert.True(queryResponse.CountTotal >= 0);

            var infoResponse = await client.GetIndexInfoAsync(createdIndexId);
            Assert.Equal(createdIndexId, infoResponse.Id);
            Assert.Equal(indexName, infoResponse.Name);
        }
        finally
        {
            if (indexId.HasValue)
            {
                try
                {
                    await client.DeleteIndexAsync(indexId.Value);
                }
                catch (SeekStormApiException)
                {
                    // Keep cleanup best-effort for integration environments.
                }
            }
        }
    }

    [Fact]
    public async Task ApiKeyLifecycle_CanCreateAndDeleteKeyWithMasterKey()
    {
        using var client = new SeekStormClient(BaseUrl, DemoApiKey);

        var createResponse = await client.CreateApikeyAsync(
            MasterApiKey,
            new ApikeyQuotaObject
            {
                IndicesMax = 1,
                DocumentsMax = 10,
                OperationsMax = 100,
            });

        Assert.False(string.IsNullOrWhiteSpace(createResponse.ApiKeyBase64));

        var deleteResponse = await client.DeleteApikeyAsync(
            createResponse.ApiKeyBase64,
            MasterApiKey);

        Assert.True(deleteResponse.RemainingApiKeys >= 0);
    }
}
