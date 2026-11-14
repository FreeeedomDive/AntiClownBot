namespace AntiClown.F1Predictions.Client;

public class F1PredictionsException(int statusCode, string title, string detail) : Exception(detail)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
}
