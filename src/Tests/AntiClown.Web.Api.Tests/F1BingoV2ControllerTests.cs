using AntiClown.F1Predictions.Client;
using AntiClown.F1Predictions.Dto;
using AntiClown.Web.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace AntiClown.Web.Api.Tests;

public class F1BingoV2ControllerTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task OwnBoard_UsesAuthenticatedParticipantAndForwardsSeasonAndCancellation(bool create)
    {
        var participantId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var client = Substitute.For<IF1PredictionsClient>();
        var details = new BingoDetailsDto(2025, 2026, false, null);
        client.GetOwnBingoAsync(participantId, 2025, cancellation.Token).Returns(details);
        client.CreateOwnBingoAsync(participantId, 2025, cancellation.Token).Returns(details);
        var controller = CreateController(client, participantId);

        var result = create
            ? await controller.CreateOwnBingo(2025, cancellation.Token)
            : await controller.GetOwnBingo(2025, cancellation.Token);

        Assert.That(result.Value, Is.SameAs(details));
        if (create)
            await client.Received(1).CreateOwnBingoAsync(participantId, 2025, cancellation.Token);
        else
            await client.Received(1).GetOwnBingoAsync(participantId, 2025, cancellation.Token);
    }

    [TestCase(401, 502)]
    [TestCase(403, 502)]
    [TestCase(409, 409)]
    [TestCase(502, 502)]
    [TestCase(504, 504)]
    public async Task UpstreamFailure_ReturnsProblemWithoutTreatingServiceAuthFailureAsUserLogout(int upstreamStatus, int status)
    {
        var client = Substitute.For<IF1PredictionsClient>();
        client.GetOwnBingoAsync(Arg.Any<Guid>(), 2026, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<BingoDetailsDto>(new F1PredictionsException(upstreamStatus, "Bingo unavailable", "Try later")));

        var result = await CreateController(client, Guid.NewGuid()).GetOwnBingo(2026, default);

        var response = (ObjectResult)result.Result!;
        var problem = (ProblemDetails)response.Value!;
        Assert.That(response.StatusCode, Is.EqualTo(status));
        Assert.That(problem.Extensions["upstreamStatus"], Is.EqualTo(upstreamStatus));
        Assert.That(problem.Detail, Is.EqualTo("Try later"));
    }

    private static F1BingoV2Controller CreateController(IF1PredictionsClient client, Guid participantId)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = $"userId={participantId}";
        return new F1BingoV2Controller(client)
        {
            ControllerContext = new ControllerContext { HttpContext = context },
        };
    }
}
