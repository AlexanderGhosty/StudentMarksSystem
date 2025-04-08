using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    /// <summary>
    /// Response class for login operations containing authentication results
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the authenticated user information
        /// </summary>
        public Models.User User { get; set; }

        /// <summary>
        /// Gets or sets the authentication token for subsequent API requests
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// Response class containing a list of users retrieved from the API
    /// </summary>
    public class UsersResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the list of users retrieved from the API
        /// </summary>
        public System.Collections.Generic.List<Client.Models.User> Users { get; set; }
    }

    /// <summary>
    /// Response class for user creation operations
    /// </summary>
    public class CreateUserResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the newly created user
        /// </summary>
        public Client.Models.User CreatedUser { get; set; }
    }

    /// <summary>
    /// Basic response class for operations that don't return specific data
    /// </summary>
    public class BasicResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Response class containing a list of subjects retrieved from the API
    /// </summary>
    public class SubjectsResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the list of subjects retrieved from the API
        /// </summary>
        public System.Collections.Generic.List<Client.Models.Subject> Subjects { get; set; }
    }

    /// <summary>
    /// Response class for subject creation operations
    /// </summary>
    public class CreateSubjectResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the newly created subject
        /// </summary>
        public Client.Models.Subject CreatedSubject { get; set; }
    }
    /// <summary>
    /// Response class containing a list of grades retrieved from the API
    /// </summary>
    public class GradesResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the list of grades retrieved from the API
        /// </summary>
        public List<Client.Models.Grade> Grades { get; set; }
    }

    /// <summary>
    /// Response class for grade creation operations
    /// </summary>
    public class CreateGradeResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the newly created grade
        /// </summary>
        public Client.Models.Grade CreatedGrade { get; set; }
    }
}
