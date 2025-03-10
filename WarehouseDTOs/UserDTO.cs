using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public RoleUser Role { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; } = null;
    }
    public class UserLogin
    {
        public string EmailAddress { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UserInfoDTO
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public int? RoleId { get; set; }
    }
    public enum RoleUser
    {
        Admin =1,
        Employee=2
    }
}
