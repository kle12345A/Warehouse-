using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.import
{
    public interface IProductImportService
    {
        Task<List<ProductImportResultDTO>> ImportProductsFromExcelAsync(Stream excelStream);
        Task<List<ProductImportDTO>> ReadExcelFileAsync(Stream fileStream);

    }
}
