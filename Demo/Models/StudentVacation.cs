using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class StudentVacation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vacation Title")]
        public string VacationTitle { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Background Color")]
        public string BackgroundColor { get; set; } = "#ffffff";

        [Required]
        [Display(Name = "Symbol")]
        public string Symbol { get; set; } = "🏖️";

        [Required]
        [Display(Name = "Campus")]
        public string Campus { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        [Display(Name = "Apply on Weekends?")]
        public bool ApplyOnWeekend { get; set; } = false;

        [Required]
        [Display(Name = "Batch")]
        public string Batch { get; set; } = string.Empty;
    }
}
