namespace Client.Services
{
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
}
