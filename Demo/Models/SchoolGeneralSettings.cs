using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class SchoolGeneralSettings
    {
        public int Id { get; set; }

        // 📌 General Information
        [Display(Name = "School Name")]
        public string? SchoolName { get; set; }

        [Display(Name = "Institution Type")]
        public string? InstitutionType { get; set; }

        public string? SchoolAbbreviation { get; set; }

        public string? SchoolTagline { get; set; }

        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanNumber { get; set; }

        [Display(Name = "School Address")]
        public string? Address { get; set; }

        // 📎 Uploaded File Paths
        public string? Logo1Path { get; set; }
        public string? Logo2Path { get; set; }
        public string? PaidStampPath { get; set; }
        public string? ReportHeaderPath { get; set; }
        public string? ReportCardBackgroundPath { get; set; }
        public string? PrincipalSignatureLogoPath { get; set; }

        // 📌 School Controls
        public string? AdmissionNoPrefix { get; set; }
        public string? AdmissionNoPostfix { get; set; }
        [Display(Name = "Institution Fee Criteria")]
        public string? InstitutionFeeCriteria { get; set; }

        [Display(Name = "Institution Fee Criteria Type")]
        public string? InstitutionFeeCriteriaType { get; set; } // "Service" or "Structure"
        public string? PreAdmissionNoPrefix { get; set; }
        public string? PreAdmissionNoPostfix { get; set; }

        public string? PaymentReceiptFormat { get; set; }
        public string? FamilyWisePaymentReceiptFormat { get; set; }
        public string? AdmissionFormFormat { get; set; }
        public string? ReceiptsAndPaymentsFormat { get; set; }

        public string? AssessmentTypeForApp { get; set; }

        public bool CreateFeeVoucherWithZeroAmount { get; set; }
        public bool InactiveBatchTimetableValidation { get; set; }

        public string? EmployeeIdPrefix { get; set; }
        public string? EmployeeIdPostfix { get; set; }
        public int? RetirementAge { get; set; }

        public string? SecurityPaymentReceiptFormat { get; set; }
        public string? GateAttendanceType { get; set; }

        // 📌 Fee Voucher for App
        public string? FeeVoucherFormat { get; set; }
        public string? ServiceType { get; set; }
        public string? BalanceChoice { get; set; }
        public string? FineType { get; set; }
        public string? DiscountType { get; set; }

        // 📩 Auto SMS & App Notifications
        public bool MessageOnFeeReceive { get; set; }
        public bool MessageOnInquiry { get; set; }
        public bool MessageOnAdmission { get; set; }
        public bool MessageOnFeeReceiptInstallment { get; set; }
        public bool MessageOnDailyAttendance { get; set; }
        public bool MessageOfAttendanceFromTeacher { get; set; }

        public bool AppNotificationOnComplainStatusChange { get; set; }
        public bool AppNotificationOnDateAttendance { get; set; }
        public bool AppNotificationOnStudentAttendanceSubjectBasis { get; set; }
        public bool SmsNotificationOnDateAttendance { get; set; }
        public bool AppNotificationOnNewAnnouncement { get; set; }
        public bool AppNotificationOnBatchActivity { get; set; }
        public bool AppNotificationOnMeeting { get; set; }
        public bool AppNotificationOnApplication { get; set; }
    }
}
