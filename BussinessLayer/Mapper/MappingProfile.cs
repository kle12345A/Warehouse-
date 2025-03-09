using AutoMapper;
using BussinessLayer.Service.order;
using DataAccessLayer.Models;
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

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductRequest>().ReverseMap();
            CreateMap<ProductDTO, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore()); // Bỏ qua ánh xạ Category
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            // Order
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDetail, OrderDetailsDTO>();



            // Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryUpdateDTO>();

           

           

            // Supplier
            CreateMap<Supplier, SupplierDTO>();
            CreateMap<SupplierDTO, Supplier>();
            CreateMap<SupplierUpdateDTO, Supplier>();
            CreateMap<Supplier, SupplierUpdateDTO>();

            // User
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();



            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<CustomerUpdateDTO, Customer>();
            CreateMap<Customer, CustomerUpdateDTO>();
        }
    }
}
