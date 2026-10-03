using System.Net;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.OpenF1;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class OpenF1StartingGridClientTests
{
    [Test]
    public async Task GetDriverNamesAsync_Should_UseQualifyingSessionForRaceRoundAndOrderByGridPosition()
    {
        var client = CreateClient(url => url switch
        {
            "/v1/sessions?year=2026&session_name=Race" => """
                [{"session_key":101,"meeting_key":1,"date_start":"2026-03-01T12:00:00Z"},
                 {"session_key":202,"meeting_key":2,"date_start":"2026-03-08T12:00:00Z"}]
                """,
            "/v1/sessions?meeting_key=2&session_name=Qualifying" => """
                [{"session_key":201,"meeting_key":2,"date_start":"2026-03-07T12:00:00Z"}]
                """,
            "/v1/starting_grid?session_key=201" => """
                [{"driver_number":22,"position":2},{"driver_number":11,"position":1}]
                """,
            "/v1/drivers?session_key=201" => """
                [{"driver_number":11,"last_name":"DriverA"},{"driver_number":22,"last_name":"DriverB"}]
                """,
            _ => throw new AssertionException($"Unexpected URL: {url}"),
        });

        var names = await client.GetDriverNamesAsync(2026, 2, false);

        names.Should().Equal("DriverA", "DriverB");
    }

    [Test]
    public async Task GetDriverNamesAsync_Should_UseSprintQualifyingFromSameMeeting()
    {
        var client = CreateClient(url => url switch
        {
            "/v1/sessions?year=2026&session_name=Race" => """
                [{"session_key":101,"meeting_key":7,"date_start":"2026-03-01T12:00:00Z"}]
                """,
            "/v1/sessions?meeting_key=7&session_name=Sprint%20Qualifying" => """
                [{"session_key":99,"meeting_key":7,"date_start":"2026-02-28T12:00:00Z"}]
                """,
            "/v1/starting_grid?session_key=99" => """
                [{"driver_number":11,"position":1}]
                """,
            "/v1/drivers?session_key=99" => """
                [{"driver_number":11,"last_name":"DriverA"}]
                """,
            _ => throw new AssertionException($"Unexpected URL: {url}"),
        });

        var names = await client.GetDriverNamesAsync(2026, 1, true);

        names.Should().Equal("DriverA");
    }

    [Test]
    public async Task GetDriverNamesAsync_Should_WaitWhenGridIsUnavailable()
    {
        var client = CreateClient(_ => null);

        var names = await client.GetDriverNamesAsync(2026, 1, false);

        names.Should().BeNull();
    }

    [Test]
    public async Task GetDriverNamesAsync_Should_SpaceFourRequestsAcrossRateLimitWindow()
    {
        var requestTimes = new List<DateTimeOffset>();
        var client = CreateClient(url => url switch
        {
            "/v1/sessions?year=2026&session_name=Race" => """
                [{"session_key":101,"meeting_key":1,"date_start":"2026-03-01T12:00:00Z"}]
                """,
            "/v1/sessions?meeting_key=1&session_name=Qualifying" => """
                [{"session_key":100,"meeting_key":1,"date_start":"2026-02-28T12:00:00Z"}]
                """,
            "/v1/starting_grid?session_key=100" => """
                [{"driver_number":11,"position":1}]
                """,
            "/v1/drivers?session_key=100" => """
                [{"driver_number":11,"last_name":"DriverA"}]
                """,
            _ => throw new AssertionException($"Unexpected URL: {url}"),
        }, () => requestTimes.Add(DateTimeOffset.UtcNow));

        var names = await client.GetDriverNamesAsync(2026, 1, false);

        names.Should().Equal("DriverA");
        requestTimes.Should().HaveCount(4);
        (requestTimes[3] - requestTimes[0]).Should().BeGreaterThanOrEqualTo(TimeSpan.FromSeconds(1));
    }

    private static OpenF1StartingGridClient CreateClient(Func<string, string?> responseForUrl, Action? onRequest = null)
    {
        var handler = new StubHandler(responseForUrl, onRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.openf1.org") };
        return new OpenF1StartingGridClient(httpClient, NullLogger<OpenF1StartingGridClient>.Instance);
    }

    private sealed class StubHandler(Func<string, string?> responseForUrl, Action? onRequest) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            onRequest?.Invoke();
            var body = responseForUrl(request.RequestUri!.PathAndQuery);
            return Task.FromResult(body is null
                ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
        }
    }
}
