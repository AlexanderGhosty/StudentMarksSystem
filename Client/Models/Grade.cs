using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Grade
    {
        public int Id { get; set; }

        [JsonProperty("student_id")]
        public int StudentId { get; set; }

        [JsonProperty("subject_id")]
        public int SubjectId { get; set; }

        [JsonProperty("grade")]
        public int GradeValue { get; set; }

        // Дополнительно для удобства отображения
        [JsonProperty("student_name")]
        public string StudentName { get; set; } = string.Empty;
        public string SubjectTitle { get; set; } = string.Empty;
    }
}
