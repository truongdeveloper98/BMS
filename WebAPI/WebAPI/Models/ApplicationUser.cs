using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }

        [StringLength(50)]
        public string First_Name { get; set; }
        [StringLength(50)]
        public string Last_Name { get; set; }

        public DateTime? Birth_Date { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string Refresh_Token { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Last_Updated { get; set; }
        [StringLength(128)]
        public string Created_By { get; set; }
        [StringLength(128)]
        public string Updated_By { get; set; }
        public string RoleId { get; set; }
        public virtual IdentityRole Role { get; set; }
        public virtual BS_UserInfo UserInfo { get; set; }
        public virtual ICollection<BS_Report> Reports { get; set; }
        public virtual ICollection<BS_ReportOff> ReportOffs { get; set; }
        public virtual ICollection<BS_Recruitment> Recruitments { get; set; }

        public virtual ICollection<BS_UserProject> UserProjects { get; set; }

        public virtual ICollection<BS_UserSalaries> UserSalaries { get; set; }


        //Các thuộc tính sau được kế thừa từ Microsoft.AspNetCore.Identity.IdentityUser
        ////
        //// Summary:
        ////     A random value that must change whenever a users credentials change (password
        ////     changed, login removed)
        //public virtual string SecurityStamp { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a flag indicating if a user has confirmed their telephone address.
        //[PersonalData]
        //public virtual bool PhoneNumberConfirmed { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a telephone number for the user.
        //[ProtectedPersonalData]
        //public virtual string PhoneNumber { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a salted and hashed representation of the password for this user.
        //public virtual string PasswordHash { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the normalized user name for this user.
        //public virtual string NormalizedUserName { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the normalized email address for this user.
        //public virtual string NormalizedEmail { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the date and time, in UTC, when any user lockout ends.
        ////
        //// Remarks:
        ////     A value in the past means the user is not locked out.
        //public virtual DateTimeOffset? LockoutEnd { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a flag indicating if the user could be locked out.
        //public virtual bool LockoutEnabled { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the primary key for this user.
        //[PersonalData]
        //public virtual TKey Id { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a flag indicating if a user has confirmed their email address.
        //[PersonalData]
        //public virtual bool EmailConfirmed { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the email address for this user.
        //[ProtectedPersonalData]
        //public virtual string Email { get; set; }
        ////
        //// Summary:
        ////     A random value that must change whenever a user is persisted to the store
        //public virtual string ConcurrencyStamp { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the number of failed login attempts for the current user.
        //public virtual int AccessFailedCount { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a flag indicating if two factor authentication is enabled for this
        ////     user.
        //[PersonalData]
        //public virtual bool TwoFactorEnabled { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the user name for this user.
        //[ProtectedPersonalData]
        //public virtual string UserName { get; set; }

    }

    
}