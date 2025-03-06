using AutoMapper;
using BussinessLayer.Service.order;
using DataAccessLayer.Models;
using DataAcessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();
            CreateMap<UpdateProductRequest, Product>();

            CreateMap<Product, UpdateProductRequest>();
            // Order
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();

            // Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryUpdateDTO>();

           

            // InventoryHistory
            CreateMap<InventoryHistory, InventoryHistoryDTO>();
            CreateMap<InventoryHistoryDTO, InventoryHistory>();

            // InventoryQuota
            //CreateMap<InventoryQuota, InventoryQuotaDTO>();
            //CreateMap<InventoryQuotaDTO, InventoryQuota>();

            // Shipping
            CreateMap<Shipping, ShippingDTO>();
            CreateMap<ShippingDTO, Shipping>();

            // Supplier
            CreateMap<Supplier, SupplierDTO>();
            CreateMap<SupplierDTO, Supplier>();
            CreateMap<SupplierUpdateDTO, Supplier>();
            CreateMap<Supplier, SupplierUpdateDTO>();

            // User
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
