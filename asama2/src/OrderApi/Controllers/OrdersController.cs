using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Services;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPricingService _pricingService;
    private readonly INotificationClient _notificationClient;

    public OrdersController(
        AppDbContext db,
        IPricingService pricingService,
        INotificationClient notificationClient)
    {
        _db = db;
        _pricingService = pricingService;
        _notificationClient = notificationClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(OrderDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
        {
            ModelState.AddModelError(nameof(dto.CustomerName), "Müşteri adı zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(dto.ProductName))
        {
            ModelState.AddModelError(nameof(dto.ProductName), "Ürün adı zorunludur.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerName = dto.CustomerName.Trim(),
            ProductName = dto.ProductName.Trim(),
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            DiscountRate = dto.DiscountRate,
            TotalPrice = _pricingService.CalculateTotal(dto),
            CreatedAt = DateTime.UtcNow
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);
        await _notificationClient.NotifyOrderCreatedAsync(order, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    private static string GenerateOrderNumber()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"SPR-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{suffix}";
    }
}
