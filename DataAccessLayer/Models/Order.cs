using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? OrderType { get; set; }

    public int? Status { get; set; }

    public int UserId { get; set; }

    public int? ShippingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Shipping? Shipping { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual User User { get; set; } = null!;
}
