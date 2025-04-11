using DataAccessLayer.Models;
using DataAccessLayer.Repository.category;
//using DataAccessLayer.Repository.order;
using DataAccessLayer.Repository.product;
using DataAccessLayer.Repository.supplier;
using DataAccessLayer.Repository.user;
using BussinessLayer.Service.category;
//using BussinessLayer.Service.order;
using BussinessLayer.Service.product;
using BussinessLayer.Service.supplier;
using BussinessLayer.Service.user;
using Microsoft.EntityFrameworkCore;

using DataAccessLayer.Repository.order;
using BussinessLayer.Mapper;
using DataAccessLayer.BaseRepository;
using WarehouseDTOs;
using BussinessLayer.Service.image;
using CloudinaryDotNet;
using System.Text.Json.Serialization;
using DataAccessLayer.Repository.orderdetail;
using BussinessLayer.Service.orderdetail;
using BussinessLayer.Service.order;
using BussinessLayer.Service.customer;
using DataAccessLayer.Repository.customer;
using BussinessLayer.Service.import;
using DataAccessLayer.Repository.role;
using BussinessLayer.Service.role;
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<WarehouseDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("MyContr")));
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
var cloudinary = new Cloudinary(new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
));
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSingleton(cloudinary);
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});// Register Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>(); 
builder.Services.AddScoped<IRoleService, RoleService>();


builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IProductImportService, ProductImportService>();


// Register Services
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");
app.UseAuthorization();
app.MapControllers();

app.Run();