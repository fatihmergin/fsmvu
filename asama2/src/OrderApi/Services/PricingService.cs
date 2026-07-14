using OrderApi.Models;

namespace OrderApi.Services;

public interface IPricingService
{
    decimal CalculateTotal(OrderDto dto);
}

public class PricingService : IPricingService
{
    public decimal CalculateTotal(OrderDto dto)
    {
        var subtotal = dto.Quantity * dto.UnitPrice;
        var total = subtotal * (1 - dto.DiscountRate);
        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
