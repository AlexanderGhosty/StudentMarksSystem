using Client.Models;
using Client.Services;

namespace Client.Services
{
    public interface IApiService
    {
        Task<LoginResponse> LoginAsync(string login, string password);

        Task<UsersResponse> GetAllUsersAsync();
        Task<CreateUserResponse> CreateUserAsync(User user);
        Task<BasicResponse> DeleteUserAsync(int userId);

        Task<SubjectsResponse> GetAllSubjectsAsync();
        Task<CreateSubjectResponse> CreateSubjectAsync(Subject subject);
        Task<BasicResponse> DeleteSubjectAsync(int subjectId);

        // И т.д. по необходимости
    }
}
