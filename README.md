# SeekStorm C# REST Client

<img src="https://raw.githubusercontent.com/SeekStorm/seekstorm_client_cs/main/assets/logo.png" width="450" alt="Logo"><br>
**C# REST client** (sync and async), for the **SeekStorm vector & lexical search server**.

seekstorm_client_cs is open source licensed under the [Apache License 2.0](https://github.com/SeekStorm/seekstorm_client_py?tab=Apache-2.0-1-ov-file#readme)

## SeekStorm REST client (C#)
[![NuGet version](https://badge.fury.io/nu/SeekStorm.Client.svg)](https://badge.fury.io/nu/SeekStorm.Client)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/seekstorm_client_cs?tab=Apache-2.0-1-ov-file#readme)

## SeekStorm REST client (Pure Python)
[![PyPI](https://img.shields.io/pypi/v/seekstorm-client-pure-py?label=PyPI)](https://pypi.org/project/seekstorm-client-pure-py/)
[![GitHub Stars](https://img.shields.io/github/stars/SeekStorm/seekstorm_client_pure_py)](https://github.com/SeekStorm/seekstorm_client_pure_py)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/seekstorm_client_pure_py?tab=Apache-2.0-1-ov-file#readme)

## SeekStorm REST client (Python wrapper via PyO3/Maturin)
[![PyPI](https://img.shields.io/pypi/v/seekstorm-client-py?label=PyPI)](https://pypi.org/project/seekstorm-client-py/)
[![GitHub Stars](https://img.shields.io/github/stars/SeekStorm/seekstorm_client_py)](https://github.com/SeekStorm/seekstorm_client_py)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/seekstorm_client_py?tab=Apache-2.0-1-ov-file#readme)

## SeekStorm REST client (Rust)
[![Crates.io](https://img.shields.io/crates/v/seekstorm_client_rs.svg)](https://crates.io/crates/seekstorm_client_rs)
[![Downloads](https://img.shields.io/crates/d/seekstorm_client_rs.svg?style=flat-square)](https://crates.io/crates/seekstorm_client_rs)
[![Documentation](https://docs.rs/seekstorm_client_rs/badge.svg)](https://docs.rs/seekstorm_client_rs)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/SeekStorm?tab=Apache-2.0-1-ov-file#readme)
[![Roadmap](https://img.shields.io/badge/Roadmap-2026-DA7F07.svg)](#roadmap)

## SeekStorm multi-tenancy search server
[![Crates.io](https://img.shields.io/crates/v/seekstorm_server.svg)](https://crates.io/crates/seekstorm_server)
[![Downloads](https://img.shields.io/crates/d/seekstorm_server.svg?style=flat-square)](https://crates.io/crates/seekstorm_server)
[![Docker](https://img.shields.io/docker/pulls/wolfgarbe/seekstorm_server)](https://hub.docker.com/r/wolfgarbe/seekstorm_server)
[![REST API Documentation](https://docs.rs/seekstorm/badge.svg)](https://seekstorm.github.io/documentation/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/SeekStorm?tab=Apache-2.0-1-ov-file#readme)
[![Roadmap](https://img.shields.io/badge/Roadmap-2026-DA7F07.svg)](#roadmap)

## SeekStorm in-process search library
[![Crates.io](https://img.shields.io/crates/v/seekstorm.svg)](https://crates.io/crates/seekstorm)
[![Downloads](https://img.shields.io/crates/d/seekstorm.svg?style=flat-square)](https://crates.io/crates/seekstorm)
[![Documentation](https://docs.rs/seekstorm/badge.svg)](https://docs.rs/seekstorm)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/SeekStorm/SeekStorm?tab=Apache-2.0-1-ov-file#readme)
[![Roadmap](https://img.shields.io/badge/Roadmap-2026-DA7F07.svg)](#roadmap)
<p>
  <a href="https://seekstorm.com">Website</a> | 
  <a href="https://seekstorm.github.io/search-benchmark-game/">Benchmark</a> | 
  <a href="https://deephn.org/">Demo</a> | 
  <a href="https://github.com/SeekStorm/seekstorm_client_py">Repository for SeekStorm Python client </a> | 
  <a href="https://github.com/SeekStorm/SeekStorm">Repository for SeekStorm library, server, Rust client </a> | 
  <a href="https://github.com/SeekStorm/SeekStorm#roadmap">Roadmap</a> | 
  <a href="https://seekstorm.com/blog/">Blog</a> | 
  <a href="https://x.com/seekstorm">X</a>
</p>


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

await client.CommitIndexAsync(createIndex.IndexId);

var result = await client.QueryIndexAsync(createIndex.IndexId, new SearchRequestObject
{
    QueryString = "hello",
    Length = 5
});

Console.WriteLine($"Total hits: {result.CountTotal}");
```

