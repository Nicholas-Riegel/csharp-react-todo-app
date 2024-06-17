using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_csharp.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool Completed { get; set; }
    }
}