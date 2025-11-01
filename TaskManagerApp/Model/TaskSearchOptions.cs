using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManager.Model
{
    public class TaskSearchOptions
    {
        public string Text { get; set; }
        public int? AssignedToUserId { get; set; }
        public TaskPriority? Priority { get; set; }
        public bool OnlyNotOverdue { get; set; }
        public TaskSortOption SortBy { get; set; } = TaskSortOption.None;
    }

  

}
