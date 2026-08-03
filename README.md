# SeekStorm C# REST Client

Typed .NET REST client for SeekStorm vector & lexical search server.

## Project

- `SeekStorm.Client` targets `net8.0`
- Uses `HttpClient` + `System.Text.Json`
- Supports sync and async methods for all Python client endpoints

## Covered Endpoints

- `GET /api/v1/live`
- `POST/DELETE/GET /api/v1/apikey`
- `POST /api/v1/index`
- `GET/DELETE/PATCH /api/v1/index/{index_id}`
- `DELETE /api/v1/index/{index_id}/doc` (clear with body `clear`)
- `POST/PATCH/DELETE /api/v1/index/{index_id}/doc`
- `GET /api/v1/index/{index_id}/doc/{doc_id}`
- `POST /api/v1/index/{index_id}/iterator`
- `POST /api/v1/index/{index_id}/query`
- `POST /api/v1/index/{index_id}/file`
- `GET /api/v1/index/{index_id}/file/{doc_id}`

## Quick Start

```csharp
using SeekStorm.Client;

var client = new SeekStormClient(
    baseUrl: "http://localhost:8080",
    apiKeyBase64: "YOUR_APIKEY_BASE64"
);

var live = await client.LiveAsync();
Console.WriteLine(live.Message);

var createIndex = await client.CreateIndexAsync(new CreateIndexRequest
{
    IndexName = "demo",
    Schema = new List<Dictionary<string, object?>>
    {
        new()
        {
            ["field"] = "title",
            ["field_type"] = "Text",
            ["store"] = true,
            ["index_lexical"] = true
        }
    }
});

await client.IndexDocumentAsync(createIndex.IndexId, new Dictionary<string, object?>
{
    ["title"] = "hello seekstorm"
});

var result = await client.QueryIndexAsync(createIndex.IndexId, new SearchRequestObject
{
    QueryString = "hello",
    Length = 5
});

Console.WriteLine($"Total hits: {result.CountTotal}");
```

