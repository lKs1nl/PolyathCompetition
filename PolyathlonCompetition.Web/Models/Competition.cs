using System.ComponentModel.DataAnnotations;

namespace PolyathlonCompetition.Web.Models
{
    public class Competition
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название соревнования обязательно")]
        [Display(Name = "Название соревнования")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Дата проведения обязательна")]
        [Display(Name = "Дата проведения")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Место проведения обязательно")]
        [Display(Name = "Место проведения")]
        public string Location { get; set; } = string.Empty;
        
        [Display(Name = "Статус")]
        public CompetitionStatus Status { get; set; }

        [Display(Name = "Максимум участников")]
        public int? MaxParticipants { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
        
        // Навигационные свойства
        public ICollection<CompetitionRegistration>? CompetitionRegistrations { get; set; } // ← НОВОЕ
    }

    public enum CompetitionStatus
    {
        [Display(Name = "Запланировано")]
        Planned,
        
        [Display(Name = "Регистрация открыта")]
        RegistrationOpen,
        
        [Display(Name = "В процессе")]
        Ongoing,
        
        [Display(Name = "Завершено")]
        Completed,
        
        [Display(Name = "Отменено")]
        Cancelled
    }
}