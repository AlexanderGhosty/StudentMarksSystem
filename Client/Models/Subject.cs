using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? TeacherId { get; set; } // Может быть null, если не назначен преподаватель
    }
}
