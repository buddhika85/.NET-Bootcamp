namespace GameStore.Api.Models;

public class CustomerBasket
{
    public Guid Id { get; set; }                                   // UserId
    public List<BasketItem> Items { get; set; } = [];            // CustomerBasket has 0 to many BasketItems        - List is recomended for entity collections in EFCore, as IEnumerble is readonly
}
