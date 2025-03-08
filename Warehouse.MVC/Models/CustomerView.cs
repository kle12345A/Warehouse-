using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class CustomerView
    {
       public List<CustomerDTO> Customers { get; set; }
        public CustomerDTO Customer { get; set; } = new CustomerDTO();

    }
}
