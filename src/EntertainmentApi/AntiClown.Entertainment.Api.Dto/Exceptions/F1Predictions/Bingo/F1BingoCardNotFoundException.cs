using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions.Bingo;

public class F1BingoCardNotFoundException(Guid id) : NotFoundException($"Card {id} not found");