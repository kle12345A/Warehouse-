using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class PdfService
    {
        public InvoiceViewModel MapToInvoiceViewModel(OrderDetailView orderDetailView)
        {
            var order = orderDetailView.OrderDetailWithCustomer;
            return new InvoiceViewModel
            {
                OrderId = order.OrderId,
                //OrderDate = order.OrderDate,
                CustomerName = order.FullName,
                CustomerPhone = order.Phone,
                CustomerEmail = order.Email,
                CustomerAddress = order.Address,
                Items = order.OrderDetails?.Select(od => new InvoiceItemViewModel
                {
                    ProductName = od.ProductName,
                    ImageUrl = od.Image,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList() ?? new List<InvoiceItemViewModel>(),
                TotalAmount = order.OrderDetails?.Sum(od => od.TotalPrice) ?? 0,
                Discount = 0,
                FinalAmount = order.OrderDetails?.Sum(od => od.TotalPrice) ?? 0
            };
        }

        public async Task<byte[]> GenerateInvoicePdf(InvoiceViewModel invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Dữ liệu hóa đơn không hợp lệ.");
            }

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Times New Roman", 20, XFontStyle.Bold);
                var fontNormal = new XFont("Times New Roman", 12, XFontStyle.Regular);
                var fontBold = new XFont("Times New Roman", 12, XFontStyle.Bold);

                // Căn giữa tiêu đề "HÓA ĐƠN XUẤT KHO"
                string title = "HÓA ĐƠN XUẤT KHO";
                var titleSize = gfx.MeasureString(title, fontTitle);
                double titleX = (page.Width - titleSize.Width) / 2; // Tính vị trí X để căn giữa
                gfx.DrawString(title, fontTitle, XBrushes.Black, new XPoint(titleX, 50));

                gfx.DrawString($"Mã đơn hàng: {invoice.OrderId}", fontNormal, XBrushes.Black, new XPoint(50, 80));

                int yPoint = 130;
                gfx.DrawString("Thông tin khách hàng", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString($"Tên: {invoice.CustomerName}", fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString($"Số điện thoại: {invoice.CustomerPhone}", fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString($"Email: {invoice.CustomerEmail}", fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString($"Địa chỉ: {invoice.CustomerAddress}", fontNormal, XBrushes.Black, new XPoint(50, yPoint));

                yPoint += 40;
                gfx.DrawString("Thông tin sản phẩm", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString("STT", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                gfx.DrawString("Sản phẩm", fontBold, XBrushes.Black, new XPoint(100, yPoint));
                gfx.DrawString("Số lượng", fontBold, XBrushes.Black, new XPoint(300, yPoint));
                gfx.DrawString("Đơn giá", fontBold, XBrushes.Black, new XPoint(400, yPoint));
                gfx.DrawString("Thành tiền", fontBold, XBrushes.Black, new XPoint(500, yPoint));

                yPoint += 20;
                const int itemsPerPage = 20;
                for (int i = 0; i < invoice.Items.Count; i++)
                {
                    if (i % itemsPerPage == 0 && i > 0)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = 50;
                        gfx.DrawString("STT", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                        gfx.DrawString("Sản phẩm", fontBold, XBrushes.Black, new XPoint(100, yPoint));
                        gfx.DrawString("Số lượng", fontBold, XBrushes.Black, new XPoint(300, yPoint));
                        gfx.DrawString("Đơn giá", fontBold, XBrushes.Black, new XPoint(400, yPoint));
                        gfx.DrawString("Thành tiền", fontBold, XBrushes.Black, new XPoint(500, yPoint));
                        yPoint += 20;
                    }

                    var item = invoice.Items[i];
                    gfx.DrawString((i + 1).ToString(), fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                    gfx.DrawString(item.ProductName, fontNormal, XBrushes.Black, new XPoint(100, yPoint));
                    gfx.DrawString(item.Quantity.ToString(), fontNormal, XBrushes.Black, new XPoint(300, yPoint));
                    gfx.DrawString($"{item.UnitPrice:N0} đ", fontNormal, XBrushes.Black, new XPoint(400, yPoint));
                    gfx.DrawString($"{item.TotalPrice:N0} đ", fontNormal, XBrushes.Black, new XPoint(500, yPoint));
                    yPoint += 20;

                   
                }

                yPoint += 20;
                gfx.DrawString("Thanh toán", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
                gfx.DrawString($"Tổng tiền ({invoice.Items.Sum(x => x.Quantity)} sản phẩm):", fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                gfx.DrawString($"{invoice.TotalAmount:N0} đ", fontNormal, XBrushes.Black, new XPoint(400, yPoint));
                yPoint += 20;
                gfx.DrawString("Chiết khấu:", fontNormal, XBrushes.Black, new XPoint(50, yPoint));
                gfx.DrawString($"{invoice.Discount:N0} đ", fontNormal, XBrushes.Black, new XPoint(400, yPoint));
                yPoint += 20;
                gfx.DrawString("Tiền thu của khách hàng:", fontBold, XBrushes.Black, new XPoint(50, yPoint));
                gfx.DrawString($"{invoice.FinalAmount:N0} đ", fontBold, XBrushes.Black, new XPoint(400, yPoint));

                using (var stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return await Task.FromResult(stream.ToArray());
                }
            }
        }
    }
}