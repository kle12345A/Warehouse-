using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Shipping
{
    public int ShippingId { get; set; }

    public string? Carrier { get; set; }

    public decimal? ShippingCost { get; set; }

    public DateTime? ShippingDate { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }

    public int? Status { get; set; }

    public string? TrackingNumber { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
