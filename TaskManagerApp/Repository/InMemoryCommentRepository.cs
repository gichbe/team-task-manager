using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamTaskManager.Model
{
    public class InMemoryCommentRepository : ICommentRepository
    {
        private readonly List<Comment> _comments = new();
        private int _nextId = 1;

        public void AddComment(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (string.IsNullOrWhiteSpace(comment.Text))
                throw new ArgumentException("Komentar ne može biti prazan.");

            comment.Id = _nextId++;
            comment.Created = DateTime.Now;

            _comments.Add(comment);
        }

        public List<Comment> GetCommentsForTask(int taskId)
        {
            return _comments.Where(c => c.TaskId == taskId)
                            .OrderBy(c => c.Created)
                            .ToList();
        }
    }
}
