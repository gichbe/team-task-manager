using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManager.Model
{
    [ExcludeFromCodeCoverage]
    public class TaskReport
    {
        public int Total { get; set; }
        public int Todo { get; set; }
        public int InProgress { get; set; }
        public int Testing { get; set; }
        public int Done { get; set; }
        public int Low { get; set; }
        public int Medium { get; set; }
        public int High { get; set; }
        public int Critical { get; set; }
        public int Overdue { get; set; }
        public List<TasksPerUser> TasksByUser { get; set; }
    }
}
