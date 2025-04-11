using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class UserView
    {
        public List<UserDTO> UserDTOs { get; set; }
        public UserDTO userDTO { get; set; }
        public List<RoleDTO> roleDTO { get; set; }
        
    }
}
