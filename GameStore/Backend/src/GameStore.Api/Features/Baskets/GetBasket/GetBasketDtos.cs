

namespace GameStore.Api.Features.Baskets.GetBasket;

public record BasketItemDto(
    Guid Id,
    string Name,
    decimal Price,
    int Quantity,
    string ImageUri);
public record BasketDto(
    Guid CustomerId,
    IEnumerable<BasketItemDto> Items)
{
    public decimal TotalAmount => Items.Sum(x => x.Price * x.Quantity);
}