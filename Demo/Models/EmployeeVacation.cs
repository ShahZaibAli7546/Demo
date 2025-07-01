using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class EmployeeVacation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vacation Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Background Color")]
        public string? BackgroundColor { get; set; }

        [Display(Name = "Symbol")]
        public string? Symbol { get; set; }

        [Display(Name = "Apply on Weekend?")]
        public bool ApplyOnWeekend { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}
