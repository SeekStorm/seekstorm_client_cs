namespace SeekStorm.Client;

public sealed class SeekStormApiException : Exception
{
    public int StatusCode { get; }

    public string ResponseBody { get; }

    public SeekStormApiException(int statusCode, string responseBody)
        : base($"SeekStorm API error {statusCode}: {responseBody}")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
