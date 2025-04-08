using Client.Models;
using Client.Services;

namespace Client.Services
{
    /// <summary>
    /// Interface for API service that handles communication with the backend server
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Authenticates a user with the specified login and password
        /// </summary>
        /// <param name="login">User login name</param>
        /// <param name="password">User password</param>
        /// <returns>Response containing authentication result and user data</returns>
        Task<LoginResponse> LoginAsync(string login, string password);

        /// <summary>
        /// Sets the authentication token for subsequent API requests
        /// </summary>
        /// <param name="token">JWT authentication token</param>
        void SetToken(string token);

        /// <summary>
        /// Retrieves all users from the system
        /// </summary>
        /// <returns>Response containing the list of users</returns>
        Task<UsersResponse> GetAllUsersAsync();

        /// <summary>
        /// Creates a new user in the system
        /// </summary>
        /// <param name="user">User data to create</param>
        /// <returns>Response containing the created user details</returns>
        Task<CreateUserResponse> CreateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the system by ID
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>Response indicating operation success or failure</returns>
        Task<BasicResponse> DeleteUserAsync(int userId);

        /// <summary>
        /// Retrieves all subjects from the system
        /// </summary>
        /// <returns>Response containing the list of subjects</returns>
        Task<SubjectsResponse> GetAllSubjectsAsync();

        /// <summary>
        /// Creates a new subject in the system
        /// </summary>
        /// <param name="subject">Subject data to create</param>
        /// <returns>Response containing the created subject details</returns>
        Task<CreateSubjectResponse> CreateSubjectAsync(Subject subject);

        /// <summary>
        /// Deletes a subject from the system by ID
        /// </summary>
        /// <param name="subjectId">ID of the subject to delete</param>
        /// <returns>Response indicating operation success or failure</returns>
        Task<BasicResponse> DeleteSubjectAsync(int subjectId);

        /// <summary>
        /// Updates the title of an existing subject
        /// </summary>
        /// <param name="subjectId">ID of the subject to update</param>
        /// <param name="newTitle">New title for the subject</param>
        /// <returns>Response indicating operation success or failure</returns>
        Task<BasicResponse> UpdateSubjectAsync(int subjectId, string newTitle);

        /// <summary>
        /// Retrieves all grades for a specific subject
        /// </summary>
        /// <param name="subjectId">ID of the subject</param>
        /// <returns>Response containing grades for the specified subject</returns>
        Task<GradesResponse> GetGradesBySubjectAsync(int subjectId);

        /// <summary>
        /// Retrieves all grades for a specific student
        /// </summary>
        /// <param name="studentId">ID of the student</param>
        /// <returns>Response containing grades for the specified student</returns>
        Task<GradesResponse> GetGradesByStudentAsync(int studentId);

        /// <summary>
        /// Adds a new grade to the system
        /// </summary>
        /// <param name="grade">Grade data to add</param>
        /// <returns>Response containing the created grade details</returns>
        Task<CreateGradeResponse> AddGradeAsync(Grade grade);

        /// <summary>
        /// Deletes a grade from the system by ID
        /// </summary>
        /// <param name="gradeId">ID of the grade to delete</param>
        /// <returns>Response indicating operation success or failure</returns>
        Task<BasicResponse> DeleteGradeAsync(int gradeId);
    }
}
