using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    class TotalExportAmountResponse
    {
        [JsonProperty("totalExportAmount")]
        public decimal TotalExportAmount { get; set; }
    }
    public class TotalCustomersResponse
    {
        public int TotalCustomers { get; set; }
    }

    public class TotalSuppliersResponse
    {
        public int TotalSuppliers { get; set; }
    }

    public class TopProductExportDto
    {
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
    }

}
