using System.ComponentModel.DataAnnotations;

namespace ERMS.Application.DTOs.TimeLogs
{
    public class LogTimeDto
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [Range(0.1, 24.0, ErrorMessage = "Hours spent must be between 0.1 and 24.0 hours.")]
        public decimal HoursSpent { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DateLogged { get; set; }
    }
}
