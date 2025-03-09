using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Images { get; set; }

    public string? Unit { get; set; }

    public int? AvailableQuantity { get; set; }

    public decimal Price { get; set; }

    public decimal CostPrice { get; set; }

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? UpdatedByNavigation { get; set; }
}
