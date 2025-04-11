using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int? RoleId { get; set; }
        public RoleUser Role { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; } = null;
    }
    public class UserLogin
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string EmailAddress { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = null!;
    }
    public class UserInfoDTO
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public enum RoleUser
    {
        Admin =1,
        Employee=2
    }
}
