using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class WeekendHalfDaySetup
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campus selection is required.")]
        [Display(Name = "Campus")]
        public int CampusId { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Weekend Days")]
        public List<DayOfWeek> WeekendDays { get; set; } = new();

        [Display(Name = "Half Day Weekdays")]
        public List<DayOfWeek> HalfDayWeekdays { get; set; } = new();

        [Display(Name = "Selected Batches")]
        public List<int> SelectedBatchIds { get; set; } = new();
    }
}
