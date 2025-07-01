namespace Demo.Models
{
    public class EmployeeLeaveManagementViewModel
    {
        public LeaveAllotment NewAllotment { get; set; } = new();
        public LeaveYear NewLeaveYear { get; set; } = new();
        public List<LeaveAllotment> LeaveAllotments { get; set; } = new();
        public List<LeaveYear> LeaveYears { get; set; } = new();
    }
}
