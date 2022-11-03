using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class UserPosition
    {
        public int PositionId { get; set; }
        public List<string> User_Id { get; set; }
    }
    public class PositionUsers
    {
        public int PositionId { get; set; }
        public string User_Id { get; set; }
    }
    public class ProjectForReportViewModel
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
    }
    public class ProjectViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Project_Name { get; set; }
        [Required]
        public string Project_Code { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public string? CustomerName { get; set; }

        public string? PartnerName { get; set; }

        public string? BacklogLink { get; set; }

        public int? CustomerId { get; set; }

        public int? PartnerId { get; set; }
        public bool? StatusCoding { get; set; }
        public int Status { get; set; }
        [Required]
        public double Revenua { get; set; }
        public double? PM_Estimate { get; set; }
        public double? Brse_Estimate { get; set; }
        public double? Comtor_Estimate { get; set; }
        public double? Tester_Estimate { get; set; }
        public double? Developer_Estimate { get; set; }
        public string ProjectType_Name { get; set; }
        [Required]
        public int ProjectTypeId { get; set; }
        public bool IsProjectPM { get; set; }
        public List<UserPosition> UserPositions { get; set; }
    }


    public class ProjectQueryParameters : QueryParametersBS
    {
        public int? Status { get; set; }
        public int? ProjectType { get; set; }
        public int? Project { get; set; }
    }

    public class ProjectTypeViewModel
    {
        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }
    }
}
