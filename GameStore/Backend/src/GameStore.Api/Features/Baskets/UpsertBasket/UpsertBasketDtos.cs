namespace GameStore.Api.Features.Baskets.UpsertBasket;

public record UpsertBasketItemDto(Guid Id, int Quantity);       // gameId and qty

public record UpsertBasketDto(IEnumerable<UpsertBasketItemDto> Items);