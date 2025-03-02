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
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int GradeValue { get; set; }

        // Дополнительно для удобства отображения
        public string StudentName { get; set; } = string.Empty;
        public string SubjectTitle { get; set; } = string.Empty;
    }
}
