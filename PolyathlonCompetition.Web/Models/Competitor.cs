using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyathlonCompetition.Web.Models
{
    public class Competitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Фамилия обязательна")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;
        
        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }
        
        [Required(ErrorMessage = "Дата рождения обязательна")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        
        [Display(Name = "Команда")]
        public string? Team { get; set; }
        
        [Display(Name = "Номер участника")]
        public string? CompetitorNumber { get; set; }
        
        [Display(Name = "Категория")]
        public string? Category { get; set; }
        
        // Навигационные свойства
        public ICollection<CompetitionResult>? CompetitionResults { get; set; }
        public ICollection<CompetitionRegistration>? CompetitionRegistrations { get; set; } // ← НОВОЕ
    }
}