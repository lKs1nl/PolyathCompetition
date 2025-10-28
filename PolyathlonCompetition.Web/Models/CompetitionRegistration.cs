using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyathlonCompetition.Web.Models
{
    public class CompetitionRegistration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CompetitionId { get; set; }

        [Required]
        public int CompetitorId { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Статус регистрации")]
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        [Display(Name = "Заметки")]
        public string? Notes { get; set; }

        // Навигационные свойства
        public Competition Competition { get; set; } = null!;
        public Competitor Competitor { get; set; } = null!;
    }

    public enum RegistrationStatus
    {
        [Display(Name = "Ожидание")]
        Pending,
        
        [Display(Name = "Подтверждена")]
        Approved,
        
        [Display(Name = "Отклонена")]
        Rejected,
        
        [Display(Name = "Отменена")]
        Cancelled
    }
}