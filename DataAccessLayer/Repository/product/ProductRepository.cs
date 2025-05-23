﻿using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.product
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly WarehouseDbContext _context;
        public ProductRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task<bool> ProductExistsByNameAsync(string name)
        {
            return await _context.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }
        public async Task<int?> GetTotalStockCountAsync()
        {
            return await _context.Products.SumAsync(p => p.AvailableQuantity);
        }
    }
    }
