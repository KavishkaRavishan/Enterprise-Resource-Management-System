using ERMS.Application.Common;
using ERMS.Application.DTOs.Attachments;

namespace ERMS.Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<ServiceResult<AttachmentDto>> UploadAttachmentAsync(Guid taskId, Stream fileStream, string fileName, string contentType, long size, Guid userId);
        Task<ServiceResult<List<AttachmentDto>>> GetAttachmentsByTaskAsync(Guid taskId);
        Task<ServiceResult<bool>> DeleteAttachmentAsync(Guid id, Guid userId, string userRole);
    }
}
