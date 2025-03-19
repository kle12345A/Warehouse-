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
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await AddAsync(user);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UserDTO userDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("User ID must be greater than 0.", nameof(id));
            }

            if (userDto.UserId != id)
            {
                throw new ArgumentException("User ID in DTO does not match the provided ID.", nameof(userDto));
            }

            var existingUser = await GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            _mapper.Map(userDto, existingUser);
            await UpdateAsync(existingUser);
            return _mapper.Map<UserDTO>(existingUser);
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
    }
}