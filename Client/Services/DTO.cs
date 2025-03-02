using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public Models.User User { get; set; }
    }

    public class UsersResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public System.Collections.Generic.List<Client.Models.User> Users { get; set; }
    }

    public class CreateUserResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public Client.Models.User CreatedUser { get; set; }
    }   
}
