using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Client.Models;
using Client.Services;
using System.Net.Http.Headers;


namespace Client.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
            // Настройка базового адреса, при необходимости
            _client.BaseAddress = new Uri("http://localhost:8080");
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<LoginResponse> LoginAsync(string login, string password)
        {
            var data = new { login, password };
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/login", content);

            var result = new LoginResponse();
            if (response.IsSuccessStatusCode)
            {
                var respJson = await response.Content.ReadAsStringAsync();
                // Сервер возвращает { success, token, errorMessage, user { ... } }
                dynamic obj = JsonConvert.DeserializeObject(respJson);
                result.IsSuccess = (bool)obj.success;
                result.ErrorMessage = (string)obj.errorMessage;

                if (result.IsSuccess)
                {
                    // Считаем токен
                    string token = (string)obj.token;
                    result.Token = token;

                    // user
                    var userObj = obj.user;
                    var user = new User
                    {
                        Id = (int)userObj.id,
                        Name = (string)userObj.name,
                        Login = (string)userObj.login,
                        Role = (string)userObj.role
                    };
                    result.User = user;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Ошибка авторизации";
            }
            return result;
        }


        public async Task<UsersResponse> GetAllUsersAsync()
        {
            var r = new UsersResponse();
            var response = await _client.GetAsync("/users");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<System.Collections.Generic.List<User>>(json);
                r.IsSuccess = true;
                r.Users = users;
            }
            else
            {
                r.IsSuccess = false;
                r.ErrorMessage = "Ошибка при получении списка пользователей";
            }
            return r;
        }

        public async Task<CreateUserResponse> CreateUserAsync(User user)
        {
            var result = new CreateUserResponse();
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/users", content);
            if (response.IsSuccessStatusCode)
            {
                var respJson = await response.Content.ReadAsStringAsync();
                var createdUser = JsonConvert.DeserializeObject<User>(respJson);
                result.IsSuccess = true;
                result.CreatedUser = createdUser;
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Ошибка при создании пользователя";
            }
            return result;
        }

        public async Task<BasicResponse> DeleteUserAsync(int userId)
        {
            var res = new BasicResponse();
            var response = await _client.DeleteAsync($"/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                res.IsSuccess = true;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = "Ошибка при удалении пользователя";
            }
            return res;
        }

        // Аналогично для предметов
        public async Task<SubjectsResponse> GetAllSubjectsAsync()
        {
            var res = new SubjectsResponse();
            var response = await _client.GetAsync("/subjects");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var subs = JsonConvert.DeserializeObject<System.Collections.Generic.List<Subject>>(json);
                res.IsSuccess = true;
                res.Subjects = subs;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = "Ошибка при получении предметов";
                res.Subjects = new List<Subject>(); // Ensure Subjects is not null
            }
            return res;
        }

        public async Task<CreateSubjectResponse> CreateSubjectAsync(Subject subject)
        {
            var result = new CreateSubjectResponse();
            var json = JsonConvert.SerializeObject(subject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/subjects", content);
            if (response.IsSuccessStatusCode)
            {
                var respJson = await response.Content.ReadAsStringAsync();
                var created = JsonConvert.DeserializeObject<Subject>(respJson);
                result.IsSuccess = true;
                result.CreatedSubject = created;
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Ошибка при создании предмета";
            }
            return result;
        }

        public async Task<BasicResponse> DeleteSubjectAsync(int subjectId)
        {
            var res = new BasicResponse();
            var response = await _client.DeleteAsync($"/subjects/{subjectId}");
            if (response.IsSuccessStatusCode)
            {
                res.IsSuccess = true;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = "Ошибка удаления предмета";
            }
            return res;
        }

        public async Task<GradesResponse> GetGradesBySubjectAsync(int subjectId)
        {
            var res = new GradesResponse();
            var response = await _client.GetAsync($"/grades?subject={subjectId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var grades = JsonConvert.DeserializeObject<List<Grade>>(json);
                res.IsSuccess = true;
                res.Grades = grades;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = $"Ошибка при получении оценок по предмету {subjectId}";
            }
            return res;
        }

        public async Task<GradesResponse> GetGradesByStudentAsync(int studentId)
        {
            var res = new GradesResponse();
            var response = await _client.GetAsync($"/grades?student={studentId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var grades = JsonConvert.DeserializeObject<List<Grade>>(json);
                res.IsSuccess = true;
                res.Grades = grades;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = $"Ошибка при получении оценок студента {studentId}";
            }
            return res;
        }

        public async Task<BasicResponse> DeleteGradeAsync(int gradeId)
        {
            var res = new BasicResponse();
            var response = await _client.DeleteAsync($"/grades/{gradeId}");
            if (response.IsSuccessStatusCode)
            {
                res.IsSuccess = true;
            }
            else
            {
                res.IsSuccess = false;
                res.ErrorMessage = "Ошибка при удалении оценки";
            }
            return res;
        }


        public async Task<CreateGradeResponse> AddOrUpdateGradeAsync(Grade grade)
        {
            // Предположим, сервер ждёт JSON вида:
            // { "student_id": <int>, "subject_id": <int>, "grade": <int> }
            // для POST /grades (upsert).

            var requestObj = new
            {
                student_id = grade.StudentId,
                subject_id = grade.SubjectId,
                grade = grade.GradeValue
            };

            var json = JsonConvert.SerializeObject(requestObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/grades", content);

            var result = new CreateGradeResponse();
            if (response.IsSuccessStatusCode)
            {
                // Допустим, сервер возвращает сохранённый объект
                // { "student_id":..., "student_name":..., "subject_id":..., "subject_title":..., "grade":... }
                // Приведём к нашей модели Grade
                var respJson = await response.Content.ReadAsStringAsync();
                var updatedGrade = JsonConvert.DeserializeObject<Grade>(respJson);

                result.IsSuccess = true;
                result.CreatedGrade = updatedGrade;
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Ошибка при добавлении/обновлении оценки";
            }
            return result;
        }

    }
}
