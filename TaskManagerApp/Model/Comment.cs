using System;

namespace TeamTaskManager.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
