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

        public string Token { get; set; }
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

    public class BasicResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SubjectsResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public System.Collections.Generic.List<Client.Models.Subject> Subjects { get; set; }
    }

    public class CreateSubjectResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public Client.Models.Subject CreatedSubject { get; set; }
    }
    public class GradesResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<Client.Models.Grade> Grades { get; set; }
    }

    public class CreateGradeResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public Client.Models.Grade CreatedGrade { get; set; }
    }
}
