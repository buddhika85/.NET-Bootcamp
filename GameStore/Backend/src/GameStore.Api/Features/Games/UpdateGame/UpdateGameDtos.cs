using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Features.Games.UpdateGame;

public record UpdateGameDto(
   [Required]
   [StringLength(50, MinimumLength = 2)]
   string Name,

   [Required]
   Guid GenreId,

   [Range(1, 100)]
   decimal Price,

   DateOnly ReleaseDate,

   [Required]
   [StringLength(500, MinimumLength = 5)]
   string Description);