using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    public class InventoryHistoryDTO
    {
        public int HistoryId { get; set; }

        public int ProductId { get; set; }

        public int? OrderDetailId { get; set; }

        public int? ChangeType { get; set; }

        public int QuantityChanged { get; set; }

        public int PreviousQuantity { get; set; }

        public int NewQuantity { get; set; }

        public DateTime? ChangeDate { get; set; }

        public int UserId { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
