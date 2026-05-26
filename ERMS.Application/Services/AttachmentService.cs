using ERMS.Application.Common;
using ERMS.Application.DTOs.Attachments;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;

namespace ERMS.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IFileStorageService _fileStorageService;

        public AttachmentService(
            IAttachmentRepository attachmentRepository, 
            ITaskRepository taskRepository, 
            IFileStorageService fileStorageService)
        {
            _attachmentRepository = attachmentRepository;
            _taskRepository = taskRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ServiceResult<AttachmentDto>> UploadAttachmentAsync(Guid taskId, Stream fileStream, string fileName, string contentType, long size, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return ServiceResult<AttachmentDto>.NotFound("Task not found");

            if (fileStream == null || size == 0)
                return ServiceResult<AttachmentDto>.Fail("Invalid file uploaded");

            // Safe size limit check, e.g. 10MB
            if (size > 10 * 1024 * 1024)
                return ServiceResult<AttachmentDto>.Fail("File size exceeds the maximum limit of 10MB");

            try
            {
                // Save under uploads/attachments folder inside wwwroot
                var filePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, "uploads/attachments");
                var attachment = new Attachment(
                    fileName, 
                    size, 
                    filePath, 
                    contentType, 
                    taskId, 
                    userId
                );

                await _attachmentRepository.AddAsync(attachment);

                var created = await _attachmentRepository.GetByIdAsync(attachment.Id);
                return ServiceResult<AttachmentDto>.Created(created!.ToDto());
            }
            catch (Exception ex)
            {
                return ServiceResult<AttachmentDto>.Fail($"Failed to upload file: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<AttachmentDto>>> GetAttachmentsByTaskAsync(Guid taskId)
        {
            var list = await _attachmentRepository.GetByTaskIdAsync(taskId);
            return ServiceResult<List<AttachmentDto>>.Ok(list.Select(a => a.ToDto()).ToList());
        }

        public async Task<ServiceResult<bool>> DeleteAttachmentAsync(Guid id, Guid userId, string userRole)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
                return ServiceResult<bool>.NotFound("Attachment not found");

            // Authorization: Admin/Manager can delete any file. Employee can only delete their own uploaded files.
            if (userRole != "Admin" && userRole != "Manager" && attachment.UploadedById != userId)
                return ServiceResult<bool>.Fail("Unauthorized: You can only delete attachments that you uploaded");

            try
            {
                // 1. Delete physical file from disk
                _fileStorageService.DeleteFile(attachment.FilePath);

                // 2. Delete database entry
                await _attachmentRepository.DeleteAsync(id);

                return ServiceResult<bool>.Ok(true, "Attachment deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Failed to delete file: {ex.Message}");
            }
        }
    }
}
