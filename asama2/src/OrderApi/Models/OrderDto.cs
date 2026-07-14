using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models;

public class OrderDto
{
    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(160)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "1")]
    public decimal DiscountRate { get; set; }
}
