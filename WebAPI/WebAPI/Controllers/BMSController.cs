//Controller chứa các chức năng chung của hệ thống
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.Models;
using WebAPI.Models.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BMSController : ControllerBaseBS
    {
        public BMSController(UsageDbContext context) : base(context)
        {

        }

        [HttpPut("initDB")]
        public async Task<IActionResult> InitDB()
        {
            var users = _db.Users.Select(x => x).ToList();
            users.ForEach(x => x.LockoutEnabled = false);

            _db.UpdateRange(users);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy kinh các tài liệu cty
        [HttpGet]
        [Route("GetDocumentLink")]
        [Authorize]
        public async Task<List<DocumentViewModel>> GetDocumentLink()
        {
            var docRef = await _db.Documents.Select(d => new DocumentViewModel()
            {
                DocumentId = d.DocumentId,
                Link = d.Link
            }).OrderBy(d => d.DocumentId).ToListAsync();

            return docRef;
        }


        //Đặt lại link tài liệu
        [HttpPut]
        [Route("SetDocumentLink")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> SetDocumentLink([FromBody] DocumentViewModel doc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var docRef = await _db.Documents.FirstOrDefaultAsync(x => x.DocumentId == doc.DocumentId);

            if (docRef == null)
                return NotFound();

            docRef.Link = doc.Link;

            _db.Update(docRef);
            await _db.SaveChangesAsync();

            return Ok();
        }


        ////Gửi Email (chức năng này đang test, chưa sử dụng)
        //[HttpGet]
        //[Route("SendEmail")]
        //[Authorize(Roles = "Manager,SysAdmin")]
        //public async Task<IActionResult> SendEmail() //chức năng này đang test
        //{
        //    // To use this API, the account to send email must enabled "Less secure app access" in Google account setting
        //    // To enable "Less secure app access", sign in to your Google account then open this link
        //    // Link: https://www.google.com/settings/security/lesssecureapps


        //    //return Ok();

        //    string to = "toannd2@beetsoft.com.vn"; //To address    
        //    string from = "ductoan.dsn@gmail.com"; //From address    
        //    MailMessage mail = new MailMessage(from, to);

        //    string mailbody = ConstMessage.ConstMessageBMS.HEADER_EMAIL;
        //    mail.Subject = ConstMessage.ConstMessageBMS.FEMALE_NAME;
        //    mail.Body = mailbody;
        //    mail.BodyEncoding = Encoding.UTF8;
        //    mail.IsBodyHtml = true;
        //    SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
        //    client.UseDefaultCredentials = false;
        //    NetworkCredential credential1 = new NetworkCredential("ductoan.dsn@gmail.com", "matkhau");
        //    client.EnableSsl = true;
        //    //client.UseDefaultCredentials = false;
        //    client.Credentials = credential1;

        //        client.Send(mail);

        //    return Ok();
        //}

        //Lấy danh sach member đang chờ việc (hiển thị ngoài dashboard)
        [HttpGet("PendingMemberList/{time}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<object> GetPendingMemberList([FromRoute] string time)
        {

            var list = await _db.Users.Where(x => x.UserInfo.IsPending && !x.IsDeleted).Select(u => new
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Team = u.UserInfo.Team,
                Level = u.UserInfo.Level,
                PendingStart = u.UserInfo.PendingStart,
                Email = u.Email,
                EffortFree = u.UserInfo.EffortFree,
                CVLink = u.UserInfo.CVLink
            }).ToListAsync();

            var res = list.GroupBy(x => x.Team).Select(u => new
            {
                Team = u.Key,
                Levels = list.Where(x => x.Team == u.Key).GroupBy(i => i.Level).Select(m => new
                {
                    Name = m.Key,
                    Members = m.Select(n => new
                    {
                        DisplayName = n.DisplayName,
                        Id = n.Id,
                        Email = n.Email,
                        PendingStart = n.PendingStart,
                        EffortFree = n.EffortFree,
                        CVLink = n.CVLink,
                    })
                })
            });

            return Ok(res);
        }

        [HttpGet("WorkingHourStat/{time}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<object> GetWorkingHourStat(string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));

            var OtherPrjId = _db.Projects.Where(p => p.ProjectName == "Other").Select(p => p.ProjectId).FirstOrDefault();

            var reportList = await _db.Reports.Where(x => x.IsDeleted == false && //khong bi xoa
                                      x.Status != Const.REPORT_STATUS_REJECTED && //khong bi reject
                                      x.WorkingDay.Date.Month == month &&
                                      x.WorkingDay.Date.Year == year)
                .Select(r => r).ToListAsync();

            if (reportList.Count() == 0)
                return new
                {
                    TotalHour = 0,
                    Other = new { Hour = 0, Percentage = 0 },
                    Project = new
                    {
                        Hour = 0,
                        Percentage = 0,
                        Working = new { Hour = 0, Percentage = 0, },
                        OT = new { Hour = 0, Percentage = 0, }
                    }
                };

            var otherHour = reportList.Where(x => x.ProjectId == OtherPrjId).Sum(x => x.WorkingHour);

            var projectHour = reportList.Where(x => x.ProjectId != OtherPrjId).Sum(x => x.WorkingHour);

            var projectWorkingHour = reportList.Where(x => x.ProjectId != OtherPrjId && x.ReportType == Const.REPORT_TYPE_NORMAL).Sum(x => x.WorkingHour);

            var projectOTHour = reportList.Where(x => x.ProjectId != OtherPrjId && x.ReportType == Const.REPORT_TYPE_OT).Sum(x => x.WorkingHour);

            var res = new
            {
                TotalHour = otherHour + projectHour,
                Other = new
                {
                    Hour = otherHour,
                    Percentage = Math.Round(otherHour * 100 / (otherHour + projectHour), 2)
                },
                Project = new
                {
                    Hour = projectHour,
                    Percentage = Math.Round(projectHour * 100 / (otherHour + projectHour), 2),
                    Working = new
                    {
                        Hour = projectWorkingHour,
                        Percentage = Math.Round(projectWorkingHour * 100 / (projectWorkingHour + projectOTHour), 2),
                    },
                    OT = new
                    {
                        Hour = projectOTHour,
                        Percentage = Math.Round(projectOTHour * 100 / (projectWorkingHour + projectOTHour), 2),
                    }
                }
            };

            return Ok(res);
        }

        //Lấy danh sách member chuẩn bị onboard (hiển thị ngoài dashboard)
        [HttpGet("getMembersOnboard/{time}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<object> getMembersOnboard([FromRoute] string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));

            DateTime currDate = DateTime.Now;
            List<BS_UserOnboard> onboardList;

            if (month == currDate.Month && year == currDate.Year)
            {
                // Get members onboard of this month and also for the next month (30 days towards)

                onboardList = await _db.UserOnboards.Where(x => x.IsDeleted == false && //khong bi xoa
                                        ((x.OnboardDate.Date.Month == month) ||
                                        (x.OnboardDate.Date >= currDate &&
                                        (x.OnboardDate.Date <= currDate.AddDays(30))
                                        )) &&
                                        x.OnboardDate.Date.Year == year).Select(u => u).OrderBy(x => x.OnboardDate).ToListAsync();

            }
            else
            {
                onboardList = await _db.UserOnboards.Where(x => x.IsDeleted == false && //khong bi xoa
                                        x.OnboardDate.Date.Month == month &&
                                        x.OnboardDate.Date.Year == year).Select(u => u).OrderBy(x => x.OnboardDate).ToListAsync();
            }

            var res = new
            {
                OnboardList = onboardList,
                ChartMembersOnboardData = onboardList.GroupBy(x => x.Position).Select(chart => new
                {
                    Position = chart.Key,
                    Count = onboardList.Where(i => i.Position == chart.Key).Select(i => i).Count()
                })
            };

            return Ok(res);
        }
    }
}
