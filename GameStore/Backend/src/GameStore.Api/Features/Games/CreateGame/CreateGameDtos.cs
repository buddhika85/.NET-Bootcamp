using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Features.Games.CreateGame;

public record CreateGameDto(
    // core DTO fields
    [Required]
    [StringLength(50, MinimumLength = 2)]
    string Name,
    [Required] Guid GenreId,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate,
    [Required]
    [StringLength(500, MinimumLength = 5)]
    string Description)
{
    // non-core DTO field
    public IFormFile? ImageFile { get; set; }
}

public record GameDetailsDto(
    Guid Id,
    string Name,
    Guid GenreId,
    decimal Price,
    DateOnly ReleaseDate,
    string ImageUri,
    string LastUpdatedBy);


public record ErrorResponseDto(
    string Message
);