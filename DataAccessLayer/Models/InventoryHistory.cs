using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class InventoryHistory
{
    public int HistoryId { get; set; }

    public int ProductId { get; set; }

    public int? ChangeType { get; set; }

    public int QuantityChanged { get; set; }

    public int PreviousQuantity { get; set; }

    public int NewQuantity { get; set; }

    public DateTime? ChangeDate { get; set; }

    public int UserId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
