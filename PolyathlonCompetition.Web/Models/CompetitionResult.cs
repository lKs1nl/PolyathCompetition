using System.ComponentModel.DataAnnotations;

namespace PolyathlonCompetition.Web.Models
{
    public class CompetitionResult
    {
        public int Id { get; set; }
        public int CompetitorId { get; set; }
        public int CompetitionId { get; set; }
        
        [Display(Name = "Бег (сек)")]
        public double? RunningTime { get; set; }
        
        [Display(Name = "Плавание (сек)")]
        public double? SwimmingTime { get; set; }
        
        [Display(Name = "Стрельба (очки)")]
        public int? ShootingScore { get; set; }
        
        [Display(Name = "Силовая гимнастика")]
        public int? StrengthScore { get; set; }
        
        [Display(Name = "Общий балл")]
        public double TotalScore { get; set; }
        
        [Display(Name = "Место")]
        public int? Place { get; set; }
        
        // Навигационные свойства
        public Competitor? Competitor { get; set; }
        public Competition? Competition { get; set; }
    }
}