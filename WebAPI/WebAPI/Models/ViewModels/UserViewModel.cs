using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public bool IsDeleted { get; set; }
        public string Role { get; set; }
        //public int TypeId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public int Department { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        //public double ThisYearDayOff { get; set; }
        //public double LastYearDayOff { get; set; }
        //public double MonthDayOff { get; set; }
        //public double UsedDayOff { get; set; }
        public UserInfoVM Info { get; set; }
    }

    public class UserInfoVM
    {
        public string UserId { get; set; }
        public string Level { get; set; }
        public int Department { get; set; }
        public string Team { get; set; }
        public bool IsPending { get; set; }
        public DateTime? PendingStart { get; set; }
        public int EffortFree { get; set; }
        public DateTime? PendingEnd { get; set; }
        public double TotalLeaveDay { get; set; }
        public double OccupiedLeaveDay { get; set; }
        public double AvaiableLeaveDay { get; set; }
        public int TypeId { get; set; }

        public string Position { get; set; }
        public int Company { get; set; }

        //public int CompanyId { get; set; }
        public string CVLink { get; set; }
    }
    public class UserQueryParameters: QueryParametersBS
    {
        public bool Status { get; set; }
        public string Department { get; set; }
    }
    public class GetUserForReport
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
    public class ResgisterViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime? Birth_Date { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public string Role { get; set; }
        public UserInfoVM Info { get; set; }
    }

    public class ChangePwViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserOnboardViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public int Position { get; set; }

        public string Language { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public DateTime OnboardDate { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserOnboardQueryParameters : QueryParametersBS
    {
    }

    public class CompanyViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } 

    }
}
