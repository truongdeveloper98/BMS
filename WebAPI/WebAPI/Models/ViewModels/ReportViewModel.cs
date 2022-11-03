using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class ReportViewModel
    {
        public int? ReportId { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public int? Rate { get; set; }
        public byte WorkingType { get; set; }
        public string Note { get; set; }
        public float Time { get; set; }
        public DateTime Day { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Department { get; set; }
    }
    public class ChangeStatusViewModel
    {
        public int Status { get; set; }
        public string Description { get; set; }
    }
    public class ReportQueryParameters : QueryParametersBS
    {
        public int ReportType { get; set; }
    }

    public class ReportRequestViewModel
    {
        public int? ReportId { get; set; }
        [Required]
        public DateTime WorkingDay { get; set; }
        public List<DateTime> WorkingDays { get; set; }
        [Required]
        public float WorkingHour { get; set; }
        public float? RateValue { get; set; }
        [Required]
        public byte ReportType { get; set; }
        [Required]
        public byte WorkingType { get; set; }
        public string Note { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int PositionId { get; set; }
    }
    public class UpdateStatusViewModel
    {
        public int Status { get; set; }
        public string Description { get; set; }
    }
    public class ProjectByUserViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class ReportOffQueryParameters : QueryParametersBS
    {
    }

    public class ReportOffViewModel
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public int? ReportOffId { get; set; }
        [Required]
        public DateTime OffDateStart { get; set; }
        [Required]
        public DateTime OffDateEnd { get; set; }
        [Required]
        public byte OffTypeId { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public double OffDay { get; set; }
    }

    public class ReportAllQueryParameters : QueryParametersBS
    {
        public int ReportType { get; set; }
        public int? ProjectId { get; set; }
        public string DisplayName { get; set; }
        public DateTime? month { get; set; }
        public int? Status { get; set; }
        public int? Department { get; set; }
    }
    public class ReportExportViewModel
    {
        public string ProjectName { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public float WorkingHour { get; set; }
        public float Rate100 { get; set; }
        public float Rate150 { get; set; }
        public float Rate200 { get; set; }

    }
    public class ExportReportVm
    {
        public string ProjectName { get; set; }
        public ReportVm report { get; set; }
    }
    public class ExportReports
    {
        public string ProjectName { get; set; }
        public List<ReportVm> reportVms { get; set; }
    }
    public class ReportVm
    {
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public float WorkingHour { get; set; }
        public float Rate100 { get; set; }
        public float Rate150 { get; set; }
        public float Rate200 { get; set; }

    }
    public class ExportReportModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public float WorkingHour { get; set; }
        public DateTime WorkingDay { get; set; }
        public byte ReportType { get; set; }
        public int Status { get; set; }
        public int DayOnMonth { get; set; }
        public float RateValue { get; set; }
        public string Email { get; set; }
        public int Department { get; set; }
        public string RoleName { get; set; }
        public int WorkingType { get; set; }
    }
    public class ExportReportOffsModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime OffDateStart { get; set; }
        public DateTime OffDateEnd { get; set; }
        public int Status { get; set; }
        public byte OffTypeId { get; set; }
    }

    public class UserReportStatistics
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string UserID { get; set; }
        public double? WorkingHourDefault { get; set; }
        public double? WorkingHour { get; set; }
        public double? OffReport { get; set; }
        public double? MissingReport { get; set; }
    }

    public class OffTypeVm
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

}
