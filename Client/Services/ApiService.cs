using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Client.Models;
using Client.Services;


namespace EducationApp.Services
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
                // Предположим, что сервер возвращает объект { success, errorMessage, user { ... } }
                dynamic obj = JsonConvert.DeserializeObject(respJson);
                result.IsSuccess = (bool)obj.success;
                result.ErrorMessage = (string)obj.errorMessage;
                if (result.IsSuccess)
                {
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
    }
}
