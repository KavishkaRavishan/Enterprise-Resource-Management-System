using ERMS.Application.Common;
using ERMS.Application.DTOs.Comments;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;

namespace ERMS.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<ServiceResult<List<CommentDto>>> GetCommentsByTaskAsync(Guid taskId)
        {
            var comments = await _commentRepository.GetByTaskIdAsync(taskId);
            return ServiceResult<List<CommentDto>>.Ok(comments.Select(c => c.ToDto()).ToList());
        }

        public async Task<ServiceResult<CommentDto>> AddCommentAsync(Guid taskId, CreateCommentDto dto, Guid authorId)
        {
            var comment = new TaskComment(dto.Content, taskId, authorId);
            await _commentRepository.AddAsync(comment);

            // Reload with author details
            var created = await _commentRepository.GetByIdAsync(comment.Id);
            return ServiceResult<CommentDto>.Created(created!.ToDto());
        }

        public async Task<ServiceResult<bool>> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return ServiceResult<bool>.NotFound("Comment not found");

            if (comment.AuthorId != userId)
                return ServiceResult<bool>.Forbidden("You can only delete your own comments");

            await _commentRepository.DeleteAsync(commentId);
            return ServiceResult<bool>.Ok(true, "Comment deleted successfully");
        }
    }
}
