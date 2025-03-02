
namespace Client.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Только при создании/редактировании
        public string Role { get; set; } = "student";        // admin/teacher/student
    }
}
