namespace GameStore.Api.Features.Games.GetGames;

public record GetGamesDto(
    string? Name = null,
    int PageNumber = 1,
    int PageSize = 5);

public record GamesPageDto(
    int TotalPages,
    IReadOnlyList<GameSummaryDto> Games);

public record GameSummaryDto(
    Guid Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate);