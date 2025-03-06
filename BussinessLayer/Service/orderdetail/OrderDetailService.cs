using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.orderdetail;
using WarehouseDTOs;

namespace BussinessLayer.Service.orderdetail
{
    public class OrderDetailService : BaseService<OrderDetail>, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        public OrderDetailService(IOrderDetailRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _orderDetailRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<OrderDetailWithSupplierDTO> GetAllAsync(int id)
        {
            return await _orderDetailRepository.GetOrderDetailsWithSupplierByOrderIdAsync(id);
        }

    }
}
