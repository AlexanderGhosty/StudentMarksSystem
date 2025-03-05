using Client.Models;
using Client.Services;

namespace Client.Services
{
    public interface IApiService
    {
        Task<LoginResponse> LoginAsync(string login, string password);
        void SetToken(string token);

        Task<UsersResponse> GetAllUsersAsync();
        Task<CreateUserResponse> CreateUserAsync(User user);
        Task<BasicResponse> DeleteUserAsync(int userId);

        Task<SubjectsResponse> GetAllSubjectsAsync();
        Task<CreateSubjectResponse> CreateSubjectAsync(Subject subject);
        Task<BasicResponse> DeleteSubjectAsync(int subjectId);


        Task<GradesResponse> GetGradesBySubjectAsync(int subjectId);
        Task<GradesResponse> GetGradesByStudentAsync(int studentId);
        Task<CreateGradeResponse> AddOrUpdateGradeAsync(Grade grade);
    }
}
