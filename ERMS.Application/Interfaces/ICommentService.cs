using ERMS.Application.Common;
using ERMS.Application.DTOs.Comments;

namespace ERMS.Application.Interfaces
{
    public interface ICommentService
    {
        Task<ServiceResult<List<CommentDto>>> GetCommentsByTaskAsync(Guid taskId);
        Task<ServiceResult<CommentDto>> AddCommentAsync(Guid taskId, CreateCommentDto dto, Guid authorId);
        Task<ServiceResult<bool>> DeleteCommentAsync(Guid commentId, Guid userId);
    }
}
