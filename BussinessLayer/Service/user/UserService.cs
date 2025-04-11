using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.user;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BussinessLayer.Service.user
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _userRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await GetAllAsync();
            if (users == null)
            {
                throw new InvalidOperationException("Failed to retrieve users.");
            }
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("User ID must be greater than 0.", nameof(id));
            }

            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            var userDto = _mapper.Map<UserDTO>(user);
            if (userDto.RoleId == null)
            {
                throw new InvalidOperationException($"RoleId for user with ID {id} could not be determined.");
            }

            return userDto;
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await AddAsync(user);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UserDTO userDto)
        {
            // Kiểm tra id hợp lệ
            if (id <= 0)
            {
                throw new ArgumentException("User ID must be greater than 0.", nameof(id));
            }

            // Kiểm tra userDto.UserId có khớp với id không
            if (userDto.UserId != id)
            {
                throw new ArgumentException("User ID in DTO does not match the provided ID.", nameof(userDto));
            }

            // Lấy người dùng hiện tại từ database
            var existingUser = await GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            // Ánh xạ các trường cần thiết từ userDto sang existingUser
            _mapper.Map(userDto, existingUser);

            // Tự động cập nhật UpdatedAt
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Cập nhật vào database
            await UpdateAsync(existingUser);

            // Ánh xạ lại sang UserDTO để trả về
            var updatedUserDto = _mapper.Map<UserDTO>(existingUser);
            return updatedUserDto;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("User ID must be greater than 0.", nameof(id));
            }

            return await DeleteAsync(id);
        }

        public async Task<UserInfoDTO?> Login(UserLogin userLogin)
        {
            if (userLogin == null || string.IsNullOrEmpty(userLogin.EmailAddress) || string.IsNullOrEmpty(userLogin.Password))
            {
                return null;
            }

            var user = await _userRepository.GetQuery()
      .FirstOrDefaultAsync(u =>
          u.Email == userLogin.EmailAddress &&
          u.Password == userLogin.Password);

            if (user != null)
            {
                return new UserInfoDTO
                {
                    UserId = user.UserId,
                    UserName = user.Username,
                    EmailAddress = user.Email,
                    RoleId = (int)user.Role,
                     RoleName = Enum.GetName(typeof(RoleUser), user.Role)

                };
            }


            return null;
        }
        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.GetQuery().CountAsync();
        }
    }
}