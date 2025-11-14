using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManager.Model
{
    [ExcludeFromCodeCoverage]
    public class TasksPerUser
    {
        public int UserId { get; set; }
        public int Count { get; set; }
    }
}
