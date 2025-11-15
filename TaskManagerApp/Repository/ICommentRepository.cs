using System.Collections.Generic;

namespace TeamTaskManager.Model
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        List<Comment> GetCommentsForTask(int taskId);
    }
}
