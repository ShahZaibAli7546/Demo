using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo.Models
{
    public class TimeTableManagement
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campus is required")]
        [Display(Name = "Campus")]
        public int CampusId { get; set; }

        [Display(Name = "Campus Name")]
        public string CampusName { get; set; } = string.Empty;

        [Display(Name = "Opening Time")]
        public TimeSpan OpeningTime { get; set; }

        [Display(Name = "Closing Time")]
        public TimeSpan ClosingTime { get; set; }

        [Display(Name = "Is Weekend")]
        public bool IsWeekend { get; set; }

        [Display(Name = "Weekend Days")]
        public string Weekends { get; set; } = string.Empty;

        [Display(Name = "Is Half Day")]
        public bool IsHalfDay { get; set; }

        [Display(Name = "Half Day Weekdays")]
        public string HalfDays { get; set; } = string.Empty;

        [Display(Name = "Half Day Weekday")]
        public DayOfWeek? HalfDayWeek { get; set; }

        [Display(Name = "Half Day Closing Time")]
        public TimeSpan? HalfDayClosingTime { get; set; }

        [Display(Name = "Late Limit (minutes)")]
        public int LateLimitMinutes { get; set; }

        [Display(Name = "Early Out Limit (minutes)")]
        public int EarlyOutLimitMinutes { get; set; }

        public List<DayOfWeek> WeekendsList =>
            (Weekends ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Enum.TryParse<DayOfWeek>(s.Trim(), out var d) ? d : (DayOfWeek?)null)
            .Where(d => d != null)
            .Select(d => d!.Value)
            .ToList();

        public List<DayOfWeek> HalfDaysList =>
            (HalfDays ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Enum.TryParse<DayOfWeek>(s.Trim(), out var d) ? d : (DayOfWeek?)null)
            .Where(d => d != null)
            .Select(d => d!.Value)
            .ToList();

        public List<DayOfWeek> ValidWeekendDays { get; set; } = new();
        public List<DayOfWeek> ValidHalfDayWeekdays { get; set; } = new();

        // ✅ Apply To
        [Display(Name = "Apply To Batch")]
        public string? SelectedBatchId { get; set; }

        [Display(Name = "Apply To Employee")]
        public string? SelectedEmployeeId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Optional helper
        public bool HasValidCampus => CampusId > 0;
    }
}
