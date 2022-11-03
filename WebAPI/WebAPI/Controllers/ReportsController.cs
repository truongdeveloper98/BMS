using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.Models;
using WebAPI.Models.ViewModels;
using WebAPI.Utils;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBaseBS
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(UsageDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetAllForExport")]
        public async Task<ActionResult> GetAllForExport()
        {
            var reports = _db.Reports.Join(_db.Projects, rp => rp.ProjectId, p => p.ProjectId, (rp, p) => new
            {
                rp = rp,
                p = p

            }).Join(_db.Positions, rpos => rpos.rp.PositionId, pos => pos.PositionId, (rpos, pos) => new
            {
                rpos = rpos,
                pos = pos
            }).Select(c => new
            {
                c.rpos.rp,
                c.pos,
                c.rpos.p
            });

            var overTimes = await reports.GroupBy(x => new { x.pos.PositionId, x.p.ProjectName, x.rp.Note, x.rp.WorkingHour })
            .Select(y => new ReportViewModel()
            {
                PositionId = y.Key.PositionId,
                ProjectName = y.Key.ProjectName,
                Note = y.Key.Note

            }).ToListAsync();

            return Ok(overTimes);
        }

        //Member lấy list report của mình
        [HttpGet]
        [Route("GetMyReports")]
        [Authorize]
        public async Task<List<ReportViewModel>> GetMyReports([FromQuery] ReportQueryParameters queryParameters)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            //lấy list report
            var reports = _db.Reports.Join(_db.Projects, rp => rp.ProjectId, p => p.ProjectId, (rp, p) => new
            {
                rp = rp,
                p = p

            }).Join(_db.Positions, rpos => rpos.rp.PositionId, pos => pos.PositionId, (rpos, pos) => new
            {
                rpos = rpos,
                pos = pos
            }).Where(x => x.rpos.rp.UserId == userid && x.rpos.rp.IsDeleted == false).Select(c => new
            {
                c.rpos.rp,
                c.pos,
                c.rpos.p
            });

            var queryable = reports.AsQueryable();
            queryable = queryable.Where(r => r.rp.ReportType == queryParameters.ReportType);
            queryable = queryable.OrderBy(x => x.p.ProjectName).OrderByDescending(x => x.rp.WorkingDay);

            //lọc theo từ khoá search
            var search = queryParameters.Search ?? "";
            queryable = queryable.Where(r => r.p.ProjectName.Contains(search) || r.pos.PositionName.Contains(search));

            HttpContext.InsertParametersPaginationInHeader(queryable);

            //Dựng view model
            var result = await queryable.Paginage(queryParameters.PaginationVm).Select(x => new ReportViewModel()
            {
                ReportId = x.rp.ReportId,
                ProjectId = x.p.ProjectId,
                ProjectName = x.p.ProjectName,
                PositionId = x.pos.PositionId,
                PositionName = x.pos.PositionName,
                Rate = (int)x.rp.RateValue,
                Note = x.rp.Note,
                Time = x.rp.WorkingHour,
                Day = x.rp.WorkingDay,
                WorkingType = x.rp.WorkingType,
                Status = x.rp.Status,
                UserId = x.rp.UserId,
                Description = x.rp.Description
            }).OrderBy(x => x.Status).ToListAsync();

            return result;
        }

        //Member lấy list report đã tạo trong ngày của mình
        [HttpGet]
        [Route("GetMyReportsToday")]
        [Authorize]
        public async Task<List<ReportViewModel>> GetMyReportsToday([FromQuery] ReportQueryParameters queryParameters)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var reports = _db.Reports.Join(_db.Projects, rp => rp.ProjectId, p => p.ProjectId, (rp, p) => new
            {
                rp = rp,
                p = p

            }).Join(_db.Positions, rpos => rpos.rp.PositionId, pos => pos.PositionId, (rpos, pos) => new
            {
                rpos = rpos,
                pos = pos
            }).Where(x => x.rpos.rp.UserId == userid && x.rpos.rp.IsDeleted == false
                        && x.rpos.rp.Date_Created.Year == DateTime.Today.Year
                        && x.rpos.rp.Date_Created.Month == DateTime.Today.Month
                        && x.rpos.rp.Date_Created.Day == DateTime.Today.Day).Select(c => new
                        {
                            c.rpos.rp,
                            c.pos,
                            c.rpos.p
                        });

            var queryable = reports.AsQueryable();
            queryable = queryable.Where(r => r.rp.ReportType == queryParameters.ReportType);
            queryable = queryable.OrderBy(x => x.p.ProjectName).OrderByDescending(x => x.rp.WorkingDay);

            //lọc theo từ khoá search
            var search = queryParameters.Search ?? "";
            queryable = queryable.Where(r => r.p.ProjectName.Contains(search) || r.pos.PositionName.Contains(search));

            HttpContext.InsertParametersPaginationInHeader(queryable);

            //Dựng view model trả về client
            var result = await queryable.Paginage(queryParameters.PaginationVm).Select(x => new ReportViewModel()
            {
                ReportId = x.rp.ReportId,
                ProjectId = x.p.ProjectId,
                ProjectName = x.p.ProjectName,
                PositionId = x.pos.PositionId,
                PositionName = x.pos.PositionName,
                Rate = (int)x.rp.RateValue,
                Note = x.rp.Note,
                Time = x.rp.WorkingHour,
                Day = x.rp.WorkingDay,
                Status = x.rp.Status,
                WorkingType = x.rp.WorkingType,
                UserId = x.rp.UserId,
                Description = x.rp.Description
            }).OrderBy(x => x.Status).ToListAsync();

            return result;
        }

        //Member lấy báo cáo nghỉ của mình
        [HttpGet]
        [Route("GetMyReportsOff")]
        [Authorize]
        public async Task<List<ReportOffViewModel>> GetMyReportsOff([FromQuery] ReportOffQueryParameters queryParameters)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var reports = _db.ReportOffs.Select(ro => ro);

            reports = reports.Where(r => r.UserId == userid && r.IsDeleted == false);
            reports = reports.OrderByDescending(x => x.OffDateStart);

            HttpContext.InsertParametersPaginationInHeader(reports);

            var reportOffs = await reports.Paginage(queryParameters.PaginationVm).Select(x => new ReportOffViewModel()
            {
                ReportOffId = x.ReportOffId,
                OffDateStart = x.OffDateStart,
                OffDateEnd = x.OffDateEnd,
                OffTypeId = x.OffTypeId,
                Note = x.Note,
                Status = x.Status,
                UserId = x.UserId,
                Description = x.Description,
                OffDay = x.OffDay
            }).OrderBy(x => x.Status).ToListAsync();

            return reportOffs;
        }

        //Member lấy báo cáo nghỉ đã tạo trong ngày của mình
        [HttpGet]
        [Route("GetMyReportsOffToday")]
        [Authorize]
        public async Task<List<ReportOffViewModel>> GetMyReportsOffToday([FromQuery] ReportOffQueryParameters queryParameters)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var reports = _db.ReportOffs.Select(ro => ro);

            reports = reports.Where(r => r.Date_Created.Day == DateTime.Today.Day && r.Date_Created.Month == DateTime.Today.Month && r.Date_Created.Year == DateTime.Today.Year && r.UserId == userid && r.IsDeleted == false);

            reports = reports.OrderByDescending(x => x.OffDateStart);

            HttpContext.InsertParametersPaginationInHeader(reports);

            var reportOffs = await reports.Paginage(queryParameters.PaginationVm).Select(x => new ReportOffViewModel()
            {
                ReportOffId = x.ReportOffId,
                OffDateStart = x.OffDateStart,
                OffDateEnd = x.OffDateEnd,
                OffTypeId = x.OffTypeId,
                Note = x.Note,
                Status = x.Status,
                UserId = x.UserId,
                Description = x.Description,
                OffDay = x.OffDay
            }).OrderBy(x => x.Status).ToListAsync();

            return reportOffs;
        }

        //Tạo report hàng loạt
        [HttpPost]
        [Route("CreateMultiReports")]
        [Authorize]
        public async Task<IActionResult> CreateMultiReports([FromForm] ReportRequestViewModel model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 1 });
            }

            //ReportType = 0 => Log time giờ làm (cần kiểm tra <= 8)
            //ReportType = 1 => log time OT, không giới hạn giờ làm
            string lstOverWkDay = "";
            string lstOverOffDay = "";
            if (model.ReportType == Const.REPORT_TYPE_NORMAL)
            {
                foreach (var day in model.WorkingDays)
                {
                    var reportTime = _db.Reports
                  .Where(r => r.UserId == userid && r.WorkingDay == day && r.ReportType == Const.REPORT_TYPE_NORMAL && r.IsDeleted == false)
                  .ToList();

                    float count = 0;
                    reportTime.ForEach(x => count += x.WorkingHour);

                    count = count + model.WorkingHour;

                    if (count > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverWkDay != "")
                            lstOverWkDay += ", ";
                        lstOverWkDay += day.ToString(Message.FORMAT_DATE);
                    }

                    double offHour = 0;
                    //Check số giừo nghỉ trong ngày của member
                    {
                        //list report off có trong ngày tạo report
                        var offReport = _db.ReportOffs
                        .Where(x => x.UserId == userid &&
                                    x.OffDateStart.Date <= day &&
                                    x.OffDateEnd.Date >= day &&
                                    x.IsDeleted == false)
                        .ToList();

                        foreach (var rp in offReport)
                        {
                            DateTime date = rp.OffDateStart;
                            DateTime endDate = rp.OffDateEnd;

                            while (date.Date <= endDate.Date)
                            {
                                if (date.Date == day.Date)
                                {
                                    TimeSpan startTime = new TimeSpan(date.Hour, date.Minute, 0);
                                    TimeSpan endTime = new TimeSpan(0, 0, 0);
                                    if (date.Date == endDate.Date)
                                    {
                                        endTime = new TimeSpan(endDate.Hour, endDate.Minute, 0);
                                    }
                                    else
                                        endTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                                    offHour += CalculateOffTme(startTime, endTime);
                                }

                                //ngay tiep theo bat dau tu 8:00
                                date = date.AddDays(1);
                                date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                            }
                        }
                    }

                    if (count > Const.TIME_WORKING_HOUR - offHour && offHour > 0)
                    {
                        if (lstOverOffDay != "")
                            lstOverOffDay += ", ";
                        lstOverOffDay += day.ToString(Message.FORMAT_DATE);
                    }
                }

            }

            if (lstOverWkDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.EXCEEDED_HOUR_REPORT
                });
            if (lstOverOffDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_LEAVE
                });
            //Thêm các report mới

            //mỗi ngày 1 report
            foreach (var day in model.WorkingDays)
            {
                var report = new BS_Report();

                report.WorkingDay = day;
                report.WorkingHour = model.WorkingHour;
                report.RateValue = (float)(model.ReportType == Const.REPORT_TYPE_OT ? model.RateValue : 100);
                report.ReportType = model.ReportType;
                report.Note = model.Note;
                report.UserId = userid;
                report.Status = Const.REPORT_STATUS_WAITING;
                report.Description = "";
                report.ProjectId = model.ProjectId;
                report.IsDeleted = false;
                report.PositionId = model.PositionId;
                report.WorkingType = model.WorkingType;

                _db.Add(report);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy list các project mà member được tham gia
        //Member chỉ log được report vào các project này
        [HttpGet]
        [Route("GetProjectsByUser")]
        [Authorize]
        public async Task<List<ProjectByUserViewModel>> GetProjectsByUser()
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var project = await _db.Projects.Join(_db.UserProjects, p => p.ProjectId, up => up.ProjectId, (p, up) => new
            {
                p = p,
                up = up
            }).Where(x => x.up.UserId == userid).Select(x => new ProjectByUserViewModel
            {
                ProjectId = x.p.ProjectId,
                ProjectName = x.p.ProjectName
            }).Distinct().ToListAsync();

            return project;
        }

        //Lấy thông tin report có id xác định
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await (from ot in _db.Reports
                                join pt in _db.Projects on ot.ProjectId equals pt.ProjectId
                                join ps in _db.Positions on ot.PositionId equals ps.PositionId
                                join u in _db.Users on ot.UserId equals u.Id
                                where ot.ReportId == id
                                select new ReportViewModel
                                {
                                    ReportId = ot.ReportId,
                                    ProjectId = pt.ProjectId,
                                    ProjectName = pt.ProjectName,
                                    PositionId = ps.PositionId,
                                    PositionName = ps.PositionName,
                                    UserId = u.Id,
                                    DisplayName = u.DisplayName,
                                    Rate = (int)ot.RateValue,
                                    Note = ot.Note,
                                    Time = ot.WorkingHour,
                                    Day = ot.WorkingDay,
                                    Description =
                                    ot.Description,
                                    WorkingType = ot.WorkingType
                                }).FirstOrDefaultAsync();

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        //Lấy thông tin OffReport có id xác định
        [HttpGet]
        [Route("GetReportOff/{id}")]
        [Authorize]
        public async Task<IActionResult> GetReportOff([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await (from ro in _db.ReportOffs
                                join u in _db.Users on ro.UserId equals u.Id
                                where ro.ReportOffId == id
                                select new ReportOffViewModel
                                {
                                    ReportOffId = ro.ReportOffId,
                                    DisplayName = u.DisplayName,
                                    OffDateStart = ro.OffDateStart,
                                    OffDateEnd = ro.OffDateEnd,
                                    OffTypeId = ro.OffTypeId,
                                    Description = ro.Description,
                                    Note = ro.Note
                                }).FirstOrDefaultAsync();
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        //Tạo report mới (working và OT)
        [HttpPost]
        [Route("CreateReport")]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] ReportRequestViewModel model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            //Kiểm tra dữ liệu có hợp lệ hay không
            if (!ModelState.IsValid)
                return BadRequest(new { code = 1 });

            //ReportType = 0 => Log time giờ làm (cần kiểm tra <= 8)
            //ReportType = 1 => log time OT, không giới hạn giờ làm
            if (model.ReportType == Const.REPORT_TYPE_NORMAL)
            {
                if (model.WorkingDay.DayOfWeek == DayOfWeek.Saturday || model.WorkingDay.DayOfWeek == DayOfWeek.Sunday)
                    return BadRequest(new
                    {
                        code = 2,
                        message = Message.DONT_CRETAE_REPORT_FOR_WEEKEN
                    });

                var reportTime = _db.Reports
               .Where(r => r.UserId == userid && r.WorkingDay == model.WorkingDay && r.ReportType == Const.REPORT_TYPE_NORMAL && r.IsDeleted == false)
               .Sum(r => r.WorkingHour);

                //Kiểm tra số giờ đã log trong ngày (điều kiện <= 8h)
                {
                    //float count = 0;
                    //foreach (var item in reportTime)
                    //{
                    //    count += item.WorkingHour;
                    //}
                    //count = count + model.WorkingHour;
                    if (reportTime + model.WorkingHour > Const.TIME_WORKING_HOUR)
                        return BadRequest(new
                        {
                            code = 2,
                            message = Message.EXCEEDED_HOUR_REPORT
                        });

                    double offHour = 0;
                    //Check số giừo nghỉ trong ngày của member
                    {
                        //list report off có trong ngày tạo report
                        var offReport = _db.ReportOffs
                        .Where(x => x.UserId == userid &&
                                    x.OffDateStart.Date <= model.WorkingDay &&
                                    x.OffDateEnd.Date >= model.WorkingDay &&
                                    x.IsDeleted == false)
                        .ToList();

                        foreach (var rp in offReport)
                        {
                            DateTime date = rp.OffDateStart;
                            DateTime endDate = rp.OffDateEnd;

                            while (date.Date <= endDate.Date)
                            {
                                if (date.Date == model.WorkingDay.Date)
                                {
                                    TimeSpan startTime = new TimeSpan(date.Hour, date.Minute, 0);
                                    TimeSpan endTime = new TimeSpan(0, 0, 0);
                                    if (date.Date == endDate.Date)
                                    {
                                        endTime = new TimeSpan(endDate.Hour, endDate.Minute, 0);
                                    }
                                    else
                                        endTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                                    offHour += CalculateOffTme(startTime, endTime);
                                }

                                //ngay tiep theo bat dau tu 8:00
                                date = date.AddDays(1);
                                date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                            }
                        }
                    }

                    if (reportTime + model.WorkingHour > Const.TIME_WORKING_HOUR - offHour && offHour > 0)
                        return BadRequest(new
                        {
                            code = 2,
                            message = "Bạn đã log nghỉ " +
                                        offHour.ToString() + "h ngày " +
                                        model.WorkingDay.ToString(Message.FORMAT_DATE)
                        });
                }

            }

            //Thêm report mới
            var report = new BS_Report();

            report.WorkingDay = model.WorkingDay;
            report.WorkingHour = model.WorkingHour;
            report.RateValue = (float)(model.ReportType == Const.REPORT_TYPE_OT ? model.RateValue : 100);
            report.ReportType = model.ReportType;
            report.Note = model.Note;
            report.UserId = userid;
            report.ProjectId = model.ProjectId;
            report.Status = Const.REPORT_STATUS_WAITING;
            report.Description = "";
            report.IsDeleted = false;
            report.PositionId = model.PositionId;
            report.WorkingType = model.WorkingType;

            _db.Add(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Sửa đổi report có id xác định
        [HttpPut]
        [Route("UpdateReport/{id}")]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] int id, [FromForm] ReportRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = 1 });

            if (id != model.ReportId)
                return BadRequest(new { code = 2 });

            var report = _db.Reports.FirstOrDefault(r => r.ReportId == id);
            if (report.Status == Const.REPORT_STATUS_APPROVED)
                return BadRequest(new
                {
                    code = 5,
                    message = Message.DONT_UPDATE_APPROVED
                }); //Đã duyệt thì  k cho sửa

            if (report == null)
                return NotFound();

            //ReportType = 0 => Log time giờ làm (cần kiểm tra <= 8)
            //ReportType = 1 => log time OT, không giới hạn giờ làm
            if (model.ReportType == Const.REPORT_TYPE_NORMAL)
            {
                if (model.WorkingDay.DayOfWeek == DayOfWeek.Saturday || model.WorkingDay.DayOfWeek == DayOfWeek.Sunday)
                    return BadRequest(new
                    {
                        code = 2,
                        message = Message.DONT_CRETAE_REPORT_FOR_WEEKEN
                    });

                var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

                var reportTime = _db.Reports
               .Where(r => r.UserId == userid && r.WorkingDay == model.WorkingDay)
               .Where(r => r.ReportId != id && r.ReportType == Const.REPORT_TYPE_NORMAL && !r.IsDeleted)
               .Sum(r => r.WorkingHour);

                if (reportTime + model.WorkingHour > Const.TIME_WORKING_HOUR)
                    return BadRequest(new
                    {
                        code = 2,
                        message = Message.EXCEEDED_HOUR_REPORT
                    });

                double offHour = 0;
                //Check số giừo nghỉ trong ngày của member
                {
                    //list report off có trong ngày tạo report
                    var offReport = _db.ReportOffs
                    .Where(x => x.UserId == userid &&
                                x.OffDateStart.Date <= model.WorkingDay &&
                                x.OffDateEnd.Date >= model.WorkingDay &&
                                x.IsDeleted == false)
                    .ToList();

                    foreach (var rp in offReport)
                    {
                        DateTime date = rp.OffDateStart;
                        DateTime endDate = rp.OffDateEnd;

                        while (date.Date <= endDate.Date)
                        {
                            if (date.Date == model.WorkingDay.Date)
                            {
                                TimeSpan startTime = new TimeSpan(date.Hour, date.Minute, 0);
                                TimeSpan endTime = new TimeSpan(0, 0, 0);
                                if (date.Date == endDate.Date)
                                {
                                    endTime = new TimeSpan(endDate.Hour, endDate.Minute, 0);
                                }
                                else
                                    endTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                                offHour += CalculateOffTme(startTime, endTime);
                            }

                            //ngay tiep theo bat dau tu 8:00
                            date = date.AddDays(1);
                            date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                        }
                    }
                }

                if (reportTime + model.WorkingHour > Const.TIME_WORKING_HOUR - offHour && offHour > 0)
                    return BadRequest(new
                    {
                        code = 2,
                        message = "Bạn đã log nghỉ " +
                                    offHour.ToString() + "h ngày " +
                                    model.WorkingDay.ToString(Message.FORMAT_DATE)
                    });
            }

            report.WorkingDay = model.WorkingDay;
            report.WorkingHour = model.WorkingHour;
            report.RateValue = (float)(model.ReportType == Const.REPORT_TYPE_OT ? model.RateValue : 100);
            report.Note = model.Note;
            report.ProjectId = model.ProjectId;
            report.PositionId = model.PositionId;
            report.WorkingType = model.WorkingType;
            report.Status = Const.REPORT_STATUS_WAITING; // trả về status = 0 : đang đợi duyệt.
            report.Description = "";

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Xoá report (nhưng vẫn giữ lại trong DB)
        [HttpPut]
        [Route("DeleteReport/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = _db.Reports.FirstOrDefault(r => r.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            //IsDeleted = 1 => report bị xoá
            if (report.Status == Const.REPORT_STATUS_APPROVED)
                return BadRequest(new
                {
                    code = 5,
                    message = Message.DONT_DELETE_APPROVED
                });

            report.IsDeleted = !report.IsDeleted;

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Tạo báo cáo nghỉ (offReport)
        [HttpPost]
        [Route("CreateReportOff")]
        [Authorize]
        public async Task<IActionResult> CreateReportOff([FromForm] ReportOffViewModel model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string lstOverWkDay = "";
            string lstOverOffDay = "";
            string lstOverWkOffDay = "";
            //Check trạng thái working & nghỉ của member
            {
                var query = _db.Reports
                    .Where(x => x.UserId == userid &&
                                x.WorkingDay.Date >= model.OffDateStart.Date &&
                                x.WorkingDay.Date <= model.OffDateEnd.Date &&
                                x.IsDeleted == false);

                DateTime date = model.OffDateStart;
                DateTime endDate = model.OffDateEnd;

                //check mỗi ngày có trong báo cáo
                while (date.Date <= endDate.Date)
                {
                    //Số giờ đã làm trong ngày hôm đó
                    var workingHourToday = query.Where(x => x.WorkingDay.Date == date.Date).Sum(x => x.WorkingHour);

                    //Đếm số giờ off theo báo cáo
                    TimeSpan startOffTimeToday = new TimeSpan(date.Hour, date.Minute, 0);
                    TimeSpan endOffTimeToday = new TimeSpan(0, 0, 0);

                    if (date.Date == endDate.Date)
                    {
                        endOffTimeToday = new TimeSpan(endDate.Hour, endDate.Minute, 0);
                    }
                    else
                        endOffTimeToday = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                    //Số giờ off trong ngày theo báo cáo
                    double offHourToday = CalculateOffTme(startOffTimeToday, endOffTimeToday);

                    //Số giờ đã off ngày hôm đó
                    double offHour = 0;

                    //Những off Report có ngày đang check
                    var offReport = _db.ReportOffs
                    .Where(x => x.UserId == userid &&
                                x.OffDateStart.Date <= date.Date &&
                                x.OffDateEnd.Date >= date.Date &&
                                x.IsDeleted == false)
                    .ToList();

                    foreach (var rp in offReport)
                    {
                        DateTime startOffDate = rp.OffDateStart;
                        DateTime endOffDate = rp.OffDateEnd;

                        while (date.Date <= endOffDate.Date)
                        {
                            //Check số giờ Off trong ngày
                            if (startOffDate.Date == date.Date)
                            {
                                TimeSpan startOffTime = new TimeSpan(date.Hour, date.Minute, 0);
                                TimeSpan endOffTime = new TimeSpan(0, 0, 0);
                                if (date.Date == endDate.Date)
                                {
                                    endOffTime = new TimeSpan(endOffDate.Hour, endOffDate.Minute, 0);
                                }
                                else
                                    endOffTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                                //Số giờ đã off ngày hôm đó
                                offHour += CalculateOffTme(startOffTime, endOffTime);

                                break;
                            }

                            //ngay tiep theo bat dau tu 8:00
                            date = date.AddDays(1);
                            date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                        }
                    }

                    //Check số giờ 
                    if (workingHourToday + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverWkDay != "")
                            lstOverWkDay += ", ";

                        lstOverWkDay += date.ToString(Message.FORMAT_DATE);
                    }
                    if (offHour + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverOffDay != "")
                            lstOverOffDay += ", ";
                        lstOverOffDay += date.ToString(Message.FORMAT_DATE);
                    }
                    if (offHour + workingHourToday + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverWkOffDay != "")
                            lstOverWkOffDay += ", ";
                        lstOverWkOffDay += date.ToString(Message.FORMAT_DATE);
                    }

                    //Check xong 1 ngay
                    //Check ngay tiep theo - bat dau tu 8:00
                    date = date.AddDays(1);
                    date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                }
            }

            if (lstOverWkDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_WORKING
                });
            else if (lstOverOffDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_LEAVE
                });
            else if (lstOverWkOffDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_WORKING_AND_OFF
                });

            //Check OK => them report moi

            //thêm báo cáo nghỉ
            var report = new BS_ReportOff();

            report.OffDateStart = model.OffDateStart;
            report.OffDateEnd = model.OffDateEnd;
            report.OffTypeId = model.OffTypeId;
            report.Note = model.Note;
            report.UserId = userid;
            report.Status = Const.REPORT_STATUS_WAITING;
            report.Description = "";
            report.IsDeleted = false;
            report.OffDay = model.OffDay;

            _db.Add(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Sửa báo cáo nghỉ
        [HttpPut]
        [Route("UpdateReportOff/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReportOff([FromRoute] int id, [FromForm] ReportOffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.ReportOffId)
            {
                return BadRequest();
            }

            var report = _db.ReportOffs.FirstOrDefault(r => r.ReportOffId == id);
            if (report == null)
            {
                return NotFound();
            }

            //Khong the update nhung report nao da duoc duyet
            if (report.Status == Const.REPORT_STATUS_APPROVED)
                return BadRequest(new { code = 5, message = Message.DONT_UPDATE_APPROVED });

            string lstOverWkDay = "";
            string lstOverOffDay = "";
            string lstOverWkOffDay = "";

            //Check trạng thái working & nghỉ của member
            {
                List<BS_Report> listReport = new List<BS_Report>();

                var query = _db.Reports
                    .Where(x => x.UserId == report.UserId &&
                                x.WorkingDay.Date >= model.OffDateStart.Date &&
                                x.WorkingDay.Date <= model.OffDateEnd.Date &&
                                x.IsDeleted == false);

                DateTime date = model.OffDateStart;
                DateTime endDate = model.OffDateEnd;

                //check mỗi ngày có trong báo cáo
                while (date.Date <= endDate.Date)
                {
                    //Đếm số giờ off theo báo cáo
                    TimeSpan startOffTimeToday = new TimeSpan(date.Hour, date.Minute, 0);
                    TimeSpan endOffTimeToday = new TimeSpan(0, 0, 0);
                    if (date.Date == endDate.Date)
                    {
                        endOffTimeToday = new TimeSpan(endDate.Hour, endDate.Minute, 0);
                    }
                    else
                        endOffTimeToday = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                    //Số giờ off theo báo cáo
                    double offHourToday = CalculateOffTme(startOffTimeToday, endOffTimeToday);

                    //Số giờ đã làm trong ngày hôm đó
                    var workingHourToday = query.Where(x => x.WorkingDay.Date == date.Date).Sum(x => x.WorkingHour);

                    //Số giờ đã off ngày hôm đó
                    double offHour = 0;

                    //Những off Report có ngày đang check
                    var offReport = _db.ReportOffs
                    .Where(x => x.UserId == report.UserId &&
                                x.OffDateStart.Date <= date.Date &&
                                x.OffDateEnd.Date >= date.Date &&
                                x.IsDeleted == false &&
                                x.ReportOffId != report.ReportOffId //loại trừ report đang được update
                                )
                    .ToList();

                    foreach (var rp in offReport)
                    {
                        DateTime startOffDate = rp.OffDateStart;
                        DateTime endOffDate = rp.OffDateEnd;
                        while (date.Date <= endOffDate.Date)
                        {
                            //Check số giờ Off trong ngày
                            if (startOffDate.Date == date.Date)
                            {
                                TimeSpan startOffTime = new TimeSpan(date.Hour, date.Minute, 0);
                                TimeSpan endOffTime = new TimeSpan(0, 0, 0);
                                if (date.Date == endDate.Date)
                                {
                                    endOffTime = new TimeSpan(endOffDate.Hour, endOffDate.Minute, 0);
                                }
                                else
                                    endOffTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);

                                //Số giờ đã off ngày hôm đó
                                offHour += CalculateOffTme(startOffTime, endOffTime);

                                break;
                            }

                            //ngay tiep theo bat dau tu 8:00
                            date = date.AddDays(1);
                            date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                        }
                    }

                    //Check số giờ 
                    if (workingHourToday + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverWkDay != "")
                            lstOverWkDay += ", ";
                        lstOverWkDay += date.ToString(Message.FORMAT_DATE);
                    }
                    if (offHour + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverOffDay != "")
                            lstOverOffDay += ", ";
                        lstOverOffDay += date.ToString(Message.FORMAT_DATE);
                    }
                    if (offHour + workingHourToday + offHourToday > Const.TIME_WORKING_HOUR)
                    {
                        if (lstOverWkOffDay != "")
                            lstOverWkOffDay += ", ";
                        lstOverWkOffDay += date.ToString(Message.FORMAT_DATE);
                    }

                    //Check ngay tiep theo - bat dau tu 8:00
                    date = date.AddDays(1);
                    date = date.Date + new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                }
            }

            if (lstOverWkDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_WORKING
                });
            else if (lstOverOffDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_LEAVE
                });
            else if (lstOverWkOffDay != "")
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_WORKING_AND_OFF
                });

            //Check OK => Cập nhật report

            report.OffDateStart = model.OffDateStart;
            report.Status = Const.REPORT_STATUS_WAITING;
            report.Description = "";
            report.OffDateEnd = model.OffDateEnd;
            report.OffTypeId = model.OffTypeId;
            report.Note = model.Note;
            report.OffDay = model.OffDay;

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Xoá báo cáo nghỉ
        [HttpPut]
        [Route("DeleteReportOff/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReportOff([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = _db.ReportOffs.FirstOrDefault(r => r.ReportOffId == id);
            if (report == null)
                return NotFound();
            if (report.Status == Const.REPORT_STATUS_APPROVED)
                return BadRequest(new
                {
                    code = 5,
                    message = Message.DONT_DELETE_APPROVED
                });

            report.IsDeleted = !report.IsDeleted;

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Duyệt report
        [HttpPut("UpdateStatus/{id}")] //approve report
        [Authorize]
        public async Task<IActionResult> ApproveReport([FromRoute] int id, [FromForm] ChangeStatusViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = _db.Reports.FirstOrDefault(r => r.ReportId == id);

            if (report == null || report.IsDeleted)
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_DELETED
                });

            //Status = {0: đợi duyệt, 1: đã duyệt, 2: từ chối }
            report.Status = model.Status;
            //Comment của quản lý về report (nếu report bị từ chối)
            report.Description = model.Description;

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Duyệt báo cáo nghỉ
        [HttpPut("UpdateStatusOff/{id}")]
        [Authorize]
        public async Task<IActionResult> PutStatusOff([FromRoute] int id, [FromForm] ChangeStatusViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = _db.ReportOffs.FirstOrDefault(r => r.ReportOffId == id);
            if (report == null || report.IsDeleted)
                return BadRequest(new
                {
                    code = 2,
                    message = Message.REPORTED_DELETED
                });

            var user = _db.Users.Where(x => x.Id == report.UserId).FirstOrDefault();

            //Check xem member vào cty đủ 5 năm chưa
            bool over5Year = (DateTime.Now.Year - user.Start_Date.Value.Year > 5 ||
                     (DateTime.Now.Year - user.Start_Date.Value.Year == 5 &&
                      DateTime.Now.Month >= user.Start_Date.Value.Month)
                      );
            double thisYearOff = over5Year ? 13 : 12;

            //trừ phép khi duyệt report
            if (report.OffTypeId == 0 && report.Status != Const.REPORT_STATUS_APPROVED && model.Status == Const.REPORT_STATUS_APPROVED)
            {
                var info = _db.UserInfos.Where(v => v.UserId == report.UserId).FirstOrDefault();
                info.OccupiedLeaveDay += report.OffDay;
                //ưu tiên trừ vào phép còn lại năm trước
                if (info.LastYearLeaveDay >= report.OffDay)
                {
                    info.LastYearLeaveDay = info.LastYearLeaveDay - report.OffDay;
                }
                else
                {
                    double minus = report.OffDay - info.LastYearLeaveDay;
                    info.LastYearLeaveDay = 0;
                    info.ThisYearLeaveDay = info.ThisYearLeaveDay - minus >= 0 ? info.ThisYearLeaveDay - minus : 0;
                }

                _db.Update(info);
            }

            //cộng lại phép khi huỷ duyệt report
            if (report.OffTypeId == 0 && report.Status == Const.REPORT_STATUS_APPROVED && model.Status != Const.REPORT_STATUS_APPROVED)
            {
                var info = _db.UserInfos.Where(v => v.UserId == report.UserId).FirstOrDefault();

                info.OccupiedLeaveDay = info.OccupiedLeaveDay - report.OffDay >= 0 ? info.OccupiedLeaveDay - report.OffDay : 0;

                //ưu tiên cộng vào phép còn lại năm nay
                if (info.ThisYearLeaveDay + report.OffDay > thisYearOff)
                {
                    double plus = (info.ThisYearLeaveDay + report.OffDay) - thisYearOff;
                    info.ThisYearLeaveDay = thisYearOff;
                    info.LastYearLeaveDay += plus;
                }
                else
                {
                    info.ThisYearLeaveDay += report.OffDay;
                }

                _db.Update(info);
            }

            //status = 1: duyệt, 2: từ chối
            report.Status = model.Status;
            report.Description = model.Description;

            _db.Update(report);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Duyệt toàn bộ report trong list id
        [HttpPut("ApproveAll")]
        [Authorize]
        public async Task<IActionResult> ApproveAll([FromBody] List<int?> listId)
        {
            foreach (int? id in listId)
            {
                var report = _db.Reports.FirstOrDefault(r => r.ReportId == id);
                if (report == null || report.IsDeleted)
                    return BadRequest(new
                    {
                        code = 2,
                        message = Message.REPORTED_DELETED
                    });

                report.Status = Const.REPORT_STATUS_APPROVED;
                report.Description = "";

                _db.Update(report);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Duyệt toàn bộ OffReport trong list id
        [HttpPut("ApproveAllOff")]
        [Authorize]
        public async Task<IActionResult> ApproveAllOff([FromBody] List<int?> listId)
        {
            foreach (int? id in listId)
            {
                var report = _db.ReportOffs.FirstOrDefault(r => r.ReportOffId == id);

                if (report == null || report.IsDeleted)
                    return BadRequest(new
                    {
                        code = 2,
                        message = Message.REPORTED_DELETED
                    });

                //trừ phép khi duyệt report
                if (report.OffTypeId == 0 && report.Status != Const.REPORT_STATUS_APPROVED)
                {
                    var info = _db.UserInfos.Where(v => v.UserId == report.UserId).FirstOrDefault();

                    info.OccupiedLeaveDay += report.OffDay;

                    //ưu tiên trừ vào phép còn lại năm trước
                    if (info.LastYearLeaveDay >= report.OffDay)
                    {
                        info.LastYearLeaveDay = info.LastYearLeaveDay - report.OffDay;
                    }
                    else
                    {
                        double minus = report.OffDay - info.LastYearLeaveDay;

                        info.LastYearLeaveDay = 0;
                        info.ThisYearLeaveDay = info.ThisYearLeaveDay - minus >= 0 ? info.ThisYearLeaveDay - minus : 0;
                    }

                    _db.Update(info);
                }

                report.Status = Const.REPORT_STATUS_APPROVED;
                report.Description = "";

                _db.Update(report);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Chức năng dành cho manager, admin hoặc PM
        //Lấy báo cáo của các member
        [HttpGet]
        [Route("GetAllReportsUsers")]
        [Authorize]
        public async Task<List<ReportViewModel>> GetAllReportsUsers([FromQuery] ReportAllQueryParameters queryParameters)
        {
            DateTime today = DateTime.Today;
            var queryable = _db.Reports.Where(rp => rp.ReportType == queryParameters.ReportType
                                    && !rp.IsDeleted).Select(rp => new ReportViewModel
                                    {
                                        ReportId = rp.ReportId,
                                        ProjectId = rp.Project.ProjectId,
                                        ProjectName = rp.Project.ProjectName,
                                        PositionId = rp.Position.PositionId,
                                        PositionName = rp.Position.PositionName,
                                        Rate = (int)rp.RateValue,
                                        Note = rp.Note,
                                        Time = rp.WorkingHour,
                                        Day = rp.WorkingDay,
                                        Status = rp.Status,
                                        DisplayName = rp.User.DisplayName,
                                        WorkingType = rp.WorkingType,
                                        UserId = rp.User.Id,
                                        Department = rp.User.UserInfo.Department,
                                        Description = rp.Description,
                                    });

            //queryable = queryable.Where(r => r.ot.ReportType == queryParameters.ReportType && r.ot.IsDeleted == false);
            queryable = queryable.OrderBy(x => x.ProjectName).OrderBy(x => x.DisplayName).OrderByDescending(x => x.Day);

            //Lấy thông tin user 
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var ur = await _userManager.FindByIdAsync(userid);
            var role = await _userManager.GetRolesAsync(ur);

            var project = (from p in _db.Projects
                           join up in _db.UserProjects on p.ProjectId equals up.ProjectId
                           where up.UserId == userid && up.PositionId == Const.POSITION_PROJECT_PM
                           select p.ProjectId).Distinct().ToList();

            //Dành cho user không phải Manager, Admin nhưng có làm PM
            if (project.Count() > 0 && role.FirstOrDefault() != BSRole.MANAGER && role.FirstOrDefault() != BSRole.SYSADMIN)
            {
                //Chỉ lấy những project mà user làm PM
                //int?[] projectId = project.Select(x => (int?)x).ToArray();
                queryable = queryable.Where(x => project.Contains((int)x.ProjectId));
            }

            //Lọc kết quả theo search key
            if (queryParameters.Department == 0) //beetsoft
            {
                queryable = queryable.Where(x => x.Department != 2);
            }
            if (queryParameters.Department == 1) //partner
            {
                queryable = queryable.Where(x => x.Department == 2);
            }
            if (queryParameters.month != null)
            {
                queryable = queryable.Where(x => x.Day.Month == queryParameters.month.Value.Month && x.Day.Year == queryParameters.month.Value.Year);
            }
            else
            {
                queryable = queryable.Where(r => r.Day.Month == today.Month && r.Day.Year == today.Year);
            }

            if (queryParameters.ProjectId != null)
            {
                queryable = queryable.Where(x => x.ProjectId == queryParameters.ProjectId);
            }

            if (queryParameters.DisplayName != null && queryParameters.DisplayName != "" && queryParameters.DisplayName.Length > 0)
            {
                queryable = queryable.Where(x => x.DisplayName == queryParameters.DisplayName);
            }
            if (queryParameters.Status != null)
            {
                queryable = queryable.Where(x => x.Status == queryParameters.Status);
            }
            HttpContext.InsertParametersPaginationInHeader(queryable);

            //Dựng model trả về client
            var result = queryable.Paginage(queryParameters.PaginationVm).Select(x => x).ToList();

            return result;
        }


        //Chức năng dành cho manager, admin hoặc PM
        //Lấy báo cáo nghỉ của các member
        [HttpGet]
        [Route("GetAllReportsOffUsers")]
        [Authorize]
        public async Task<List<ReportOffViewModel>> GetAllReportsOffUsers([FromQuery] ReportAllQueryParameters queryParameters)
        {
            DateTime today = DateTime.Today;

            var reports = _db.ReportOffs.Join(_db.Users, ro => ro.UserId, u => u.Id, (ro, u) =>
                new { ro = ro, u = u })
                .Join(_db.UserInfos, ui => ui.u.Id, uinfor => uinfor.UserId, (ui, uinfor) => new
                {
                    ui = ui,
                    uinfor = uinfor
                }).Select(c => new
                {
                    c.uinfor,
                    c.ui.u,
                    c.ui.ro
                });

            if (queryParameters.ProjectId != null)
            {
                reports = from ro in reports
                          join p in _db.UserProjects on queryParameters.ProjectId equals p.ProjectId
                          where ro.ro.UserId == p.UserId
                          select ro;

            }

            var queryable = reports.AsQueryable();
            queryable = queryable.Where(r => r.ro.IsDeleted == false);
            queryable = queryable.OrderBy(x => x.u.DisplayName).OrderByDescending(x => x.ro.OffDateStart);

            //Lấy thông tin user
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var ur = await _userManager.FindByIdAsync(userid);
            var role = await _userManager.GetRolesAsync(ur);

            var project = _db.Projects.Join(_db.UserProjects, p => p.ProjectId, up => up.ProjectId, (p, up) => new
            {
                p = p,
                up = up
            }).Select(x => new ProjectByUserViewModel
            {
                ProjectId = x.p.ProjectId,
                ProjectName = x.p.ProjectName
            }).Distinct().ToList();

            //Nếu user làm PM và k có quyền manager, admin
            if (project.Count() > 0 && role.FirstOrDefault() != BSRole.MANAGER && role.FirstOrDefault() != BSRole.SYSADMIN)
            {
                //Lọc những project và user làm PM
                int[] proId = project.Select(x => (int)x.ProjectId).ToArray();
                var users = _db.UserProjects.Where(x => proId.Contains(x.ProjectId)).Select(x => new { x.UserId }).Distinct().ToList();
                string[] userId = users.Select(x => (string)x.UserId).ToArray();

                queryable = queryable.Where(x => userId.Contains(x.ro.UserId));
            }

            //Lọc theo search key
            if (queryParameters.Department == 0) //beetsoft
            {
                queryable = queryable.Where(x => x.uinfor.Department != 2);
            }
            if (queryParameters.Department == 1) //partner
            {
                queryable = queryable.Where(x => x.uinfor.Department == 2);
            }
            if (queryParameters.month != null)
            {
                queryable = queryable.Where(x => x.ro.OffDateStart.Month == queryParameters.month.Value.Month && x.ro.OffDateStart.Year == queryParameters.month.Value.Year);
            }
            else
            {
                queryable = queryable.Where(r => r.ro.OffDateStart.Month == today.Month && r.ro.OffDateStart.Year == today.Year);
            }

            if (queryParameters.DisplayName != null && queryParameters.DisplayName != "" && queryParameters.DisplayName.Length > 0)
            {
                queryable = queryable.Where(x => x.u.DisplayName == queryParameters.DisplayName);
            }
            if (queryParameters.Status != null)
            {
                queryable = queryable.Where(x => x.ro.Status == queryParameters.Status);
            }
            HttpContext.InsertParametersPaginationInHeader(queryable);

            //Dựng model trả về client
            var result = queryable.Paginage(queryParameters.PaginationVm).Select(x => new ReportOffViewModel()
            {
                ReportOffId = x.ro.ReportOffId,
                OffDateStart = x.ro.OffDateStart,
                OffDateEnd = x.ro.OffDateEnd,
                OffTypeId = x.ro.OffTypeId,
                Note = x.ro.Note,
                Status = x.ro.Status,
                DisplayName = x.u.DisplayName,
                UserId = x.u.Id,
                Description = x.ro.Description
            }).Distinct().ToList();

            return result;
        }

        //Kiểm tra tình hình log report của member trong tháng
        [HttpGet]
        [Route("CheckReports/{time}/{id}")]
        [Authorize]
        public async Task<IEnumerable<UserReportStatistics>> CheckReport([FromRoute] string time, [FromRoute] string id)
        {
            //Lấy thông tin ngày tháng
            string month = time.Substring(0, time.Length - 4);
            string years = time.Substring(time.Length - 4);

            //Check trong 1 project nhất định
            int projectId = Convert.ToInt32(id);

            var days = System.DateTime.DaysInMonth(int.Parse(years), int.Parse(month));
            var lastDayOfMonth = new DateTime();
            var firstDayOfMonth = new DateTime();

            if (DateTime.Now.Year == Int32.Parse(years) && DateTime.Now.Month == Int32.Parse(month))
            {
                lastDayOfMonth = DateTime.Now;
                firstDayOfMonth = new DateTime(lastDayOfMonth.Year, lastDayOfMonth.Month, 1);
            }
            else
            {
                firstDayOfMonth = new DateTime(Int32.Parse(years), Int32.Parse(month), 1);
                lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            }

            double count = 0;
            for (DateTime date = firstDayOfMonth; date.Date <= lastDayOfMonth.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }

            double tottalHour = count * Const.TIME_WORKING_HOUR;
            List<UserReportStatistics> UserReportStatistics = new List<UserReportStatistics>();

            //projectId = -1 => check toàn bộ
            if (projectId == -1)
            {
                UserReportStatistics = _userManager.Users
                .Where(x => x.IsDeleted == false && x.UserName != "SysAdmin")
                .Select(x => new UserReportStatistics
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    UserID = x.Id,
                    WorkingHourDefault = tottalHour,
                    WorkingHour = 0
                }).ToList();
            }
            //projectId != -1 => check trong project có id đó
            else
            {
                UserReportStatistics = _db.UserProjects.Join(_db.Users, up => up.UserId, u => u.Id, (up, u) => new
                {
                    up = up,
                    u = u
                }).Where(x => x.up.ProjectId == projectId).Select(i => new UserReportStatistics
                {
                    DisplayName = i.u.DisplayName,
                    Email = i.u.Email,
                    UserID = i.u.Id,
                    WorkingHourDefault = tottalHour,
                    WorkingHour = 0
                }).Distinct().ToList();
            }

            var userTotalHourOfWorking = _db.Reports.Join(_db.Users, r => r.UserId, u => u.Id, (r, u) => new
            {
                r = r,
                u = u
            }).Where(x => x.r.ReportType == Const.REPORT_TYPE_NORMAL
                && x.r.WorkingDay.Day >= firstDayOfMonth.Day && x.r.WorkingDay.Day <= lastDayOfMonth.Day
                && x.r.IsDeleted == false && x.r.Status == Const.REPORT_STATUS_APPROVED && x.u.IsDeleted == false)
            .GroupBy(i => i.r.UserId).Select(ur => new
            {
                TotalHour = ur.Sum(b => b.r.WorkingHour),
                UserId = ur.Key
            }).ToList();

            //Đếm giờ làm việc của member
            foreach (var statistics in UserReportStatistics)
            {
                foreach (var item in userTotalHourOfWorking)
                {
                    if (statistics.UserID == item.UserId)
                    {
                        statistics.WorkingHour = (double)item.TotalHour;
                    }
                }
            }

            //Check báo cáo nghỉ
            var listReportOffs = _db.Users.Join(_db.ReportOffs, u => u.Id, ro => ro.UserId, (u, ro) => new
            {
                u = u,
                ro = ro
            }).Where(x => x.u.IsDeleted == false && x.ro.IsDeleted == false && x.ro.Status == Const.REPORT_STATUS_APPROVED
                                 && x.ro.OffDateStart.Month == int.Parse(month)).Select(x => x.ro).ToList();

            //Tính giờ làm + nghỉ
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            foreach (var item in UserReportStatistics)
            {
                var userReportOffData = listReportOffs.Where(x => x.UserId == item.UserID).ToList();
                double sumOffWorks = 0;
                foreach (var timeBreak in userReportOffData)
                {
                    if (timeBreak.OffDateStart.Day == timeBreak.OffDateEnd.Day)
                    {
                        if (myCal.GetDayOfWeek(timeBreak.OffDateStart) != DayOfWeek.Sunday && myCal.GetDayOfWeek(timeBreak.OffDateStart) != DayOfWeek.Saturday)
                        {
                            sumOffWorks += new ReportHandler().CalculationTime(timeBreak.OffDateStart.TimeOfDay, timeBreak.OffDateEnd.TimeOfDay);
                        }
                    }
                    else
                    {
                        if (timeBreak.OffDateEnd.Month != int.Parse(month) && timeBreak.OffDateEnd.Year != int.Parse(years))
                        {
                            timeBreak.OffDateEnd = new DateTime(int.Parse(years), int.Parse(month), days, Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                        }
                        foreach (DateTime day in new ReportHandler().EachDay(timeBreak.OffDateStart, timeBreak.OffDateEnd))
                        {

                            if (myCal.GetDayOfWeek(day) != DayOfWeek.Sunday && myCal.GetDayOfWeek(day) != DayOfWeek.Saturday)
                            {
                                var timeCheckIn = new TimeSpan();
                                var timeCheckOut = new TimeSpan();
                                if (day.Day == timeBreak.OffDateStart.Day)
                                {
                                    timeCheckIn = timeBreak.OffDateStart.TimeOfDay;
                                    timeCheckOut = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                                }
                                else
                                {
                                    if (day.Day == timeBreak.OffDateEnd.Day)
                                    {
                                        timeCheckIn = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                                        timeCheckOut = timeBreak.OffDateEnd.TimeOfDay;
                                    }
                                    else
                                    {
                                        timeCheckIn = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                                        timeCheckOut = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                                    }
                                }
                                sumOffWorks += new ReportHandler().CalculationTime(timeCheckIn, timeCheckOut);
                            }
                        }
                    }
                }

                item.OffReport = sumOffWorks;
                item.MissingReport = item.WorkingHourDefault - item.OffReport - item.WorkingHour;
            }

            return UserReportStatistics.Where(x => x.MissingReport > 0).ToList();
        }

        //Xuất excel log time theo project
        [HttpGet("ExportByProject/{date}")]
        [Authorize]
        public async Task<IActionResult> Export([FromRoute] string date)
        {
            int Month = int.Parse(date.Substring(0, date.Length - 4));
            int Year = int.Parse(date.Substring(date.Length - 4));
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var user = (from u in _db.Users where u.Id == userid select u).FirstOrDefault();
            var role = await _userManager.GetRolesAsync(user);
            List<ExportReportVm> reportvm = new List<ExportReportVm>();

            //Manager hoặc Admin được export toàn bộ data
            #region for_manager
            if (role.Contains(BSRole.MANAGER) || role.Contains(BSRole.SYSADMIN))
            {
                var reports = from ot in _db.Reports
                              join pt in _db.Projects on ot.ProjectId equals pt.ProjectId
                              join ps in _db.Positions on ot.PositionId equals ps.PositionId
                              join u in _db.Users on ot.UserId equals u.Id
                              join uj in _db.UserRoles on u.Id equals uj.UserId
                              join r in _db.Roles on uj.RoleId equals r.Id
                              where ot.WorkingDay.Month == Month &&
                                    ot.WorkingDay.Year == Year &&
                                    u.IsDeleted == false &&
                                    ot.IsDeleted == false &&
                                    ot.Status == Const.REPORT_STATUS_APPROVED
                              group new { ot, u, pt, ps } by new { u.DisplayName, pt.ProjectName, r.Name } into report
                              select new
                              {
                                  report = report.Key,
                                  DisplayName = report.Key.DisplayName,
                                  WorkingHour = report.Where(x => x.ot.ReportType == Const.REPORT_TYPE_NORMAL).Sum(x => x.ot.WorkingHour),
                                  Rate100 = report.Where(x => x.ot.ReportType == Const.REPORT_TYPE_OT && x.ot.RateValue == 100).Sum(x => x.ot.WorkingHour),
                                  Rate150 = report.Where(x => x.ot.ReportType == Const.REPORT_TYPE_OT && x.ot.RateValue == 150).Sum(x => x.ot.WorkingHour),
                                  Rate200 = report.Where(x => x.ot.ReportType == Const.REPORT_TYPE_OT && x.ot.RateValue == 200).Sum(x => x.ot.WorkingHour),
                                  ProjectName = report.Key.ProjectName,
                                  Role = report.Key.Name

                              };
                reportvm = await reports.Select(x => new ExportReportVm()
                {
                    ProjectName = x.report.ProjectName,
                    report = new ReportVm()
                    {
                        DisplayName = x.DisplayName,
                        Role = x.Role,
                        WorkingHour = x.WorkingHour,
                        Rate100 = x.Rate100,
                        Rate150 = x.Rate150,
                        Rate200 = x.Rate200
                    }
                }).ToListAsync();
            }
            #endregion

            //Member không phải manager hoặc admin được export những project do mình làm PM
            #region for_PM
            else
            {
                var reportList = await (from up in _db.UserProjects
                                        join rp in _db.Reports on up.ProjectId equals rp.ProjectId
                                        where up.UserId == userid &&
                                              up.PositionId == Const.POSITION_PROJECT_PM &&
                                              rp.Date_Created.Month == Month &&
                                              rp.Date_Created.Year == Year &&
                                              rp.IsDeleted == false &&
                                              rp.Status == Const.REPORT_STATUS_APPROVED
                                        select rp).OrderBy(rp => rp.ProjectId).ThenBy(rp => rp.UserId).ToListAsync();

                BS_Report currentReport = new BS_Report();
                for (int i = 0; i < reportList.Count; i++)
                {
                    currentReport = reportList[i];
                    var ur = await _userManager.FindByIdAsync(currentReport.UserId);
                    var roles = await _userManager.GetRolesAsync(ur);
                    ExportReportVm rpvm = new ExportReportVm()
                    {
                        ProjectName = (from pj in _db.Projects where pj.ProjectId == currentReport.ProjectId select pj.ProjectName).FirstOrDefault(),
                        report = new ReportVm()
                        {
                            DisplayName = ur.DisplayName,
                            Role = roles[0],
                            WorkingHour = 0,
                            Rate100 = 0,
                            Rate150 = 0,
                            Rate200 = 0
                        }
                    };

                    while (reportList[i].ProjectId == currentReport.ProjectId && reportList[i].UserId == currentReport.UserId && i < reportList.Count)
                    {
                        if (reportList[i].ReportType == Const.REPORT_TYPE_NORMAL)
                            rpvm.report.WorkingHour += reportList[i].WorkingHour;
                        else
                        {
                            if (reportList[i].RateValue == 100)
                                rpvm.report.Rate100 += reportList[i].WorkingHour;
                            if (reportList[i].RateValue == 150)
                                rpvm.report.Rate150 += reportList[i].WorkingHour;
                            if (reportList[i].RateValue == 200)
                                rpvm.report.Rate200 += reportList[i].WorkingHour;
                        }
                        i++;
                        if (i >= reportList.Count) break;
                    }
                    reportvm.Add(rpvm);
                    if (i >= reportList.Count) break;
                    i--;

                }
            }
            #endregion

            #region another way to get list report
            //var reportList = await (from rp in _db.Reports
            //                 where    rp.Date_Created.Month == Month 
            //                       && rp.Date_Created.Year == Year
            //                       && rp.IsDeleted == false
            //                       && rp.Status == 1
            //                 select rp).OrderBy(rp => rp.ProjectId).ThenBy(rp => rp.UserId).ToListAsync();
            //
            //List<ExportReportVm> reportVmList = new List<ExportReportVm>();
            //BS_Report currentReport = new BS_Report();
            //for (int i = 0; i < reportList.Count; i++)
            //{
            //    currentReport = reportList[i];
            //    ExportReportVm rpvm = new ExportReportVm();
            //    rpvm.report = new ReportVm();
            //    rpvm.ProjectName = (from pj in _db.Projects where pj.ProjectId == currentReport.ProjectId select pj.ProjectName).FirstOrDefault();
            //    var ur = await _userManager.FindByIdAsync(currentReport.UserId);
            //    var roles = await _userManager.GetRolesAsync(ur);
            //    rpvm.report.DisplayName = ur.DisplayName;
            //    rpvm.report.Role = roles[0];
            //    rpvm.report.WorkingHour = 0;
            //    rpvm.report.Rate100 = 0;
            //    rpvm.report.Rate150 = 0;
            //    rpvm.report.Rate200 = 0;
            //    while (reportList[i].ProjectId == currentReport.ProjectId && reportList[i].UserId == currentReport.UserId && i < reportList.Count)
            //    {
            //        if(reportList[i].ReportType == 0)
            //            rpvm.report.WorkingHour += reportList[i].WorkingHour;
            //        else
            //        {
            //            if (reportList[i].RateValue == 100)
            //                rpvm.report.Rate100 += reportList[i].WorkingHour;
            //            if (reportList[i].RateValue == 150)
            //                rpvm.report.Rate150 += reportList[i].WorkingHour;
            //            if (reportList[i].RateValue == 200)
            //                rpvm.report.Rate200 += reportList[i].WorkingHour;
            //        }
            //        i++;
            //        if (i >= reportList.Count) break;
            //    }
            //    reportVmList.Add(rpvm);
            //    if (i >= reportList.Count) break;
            //    i--;
            //}
            #endregion

            var reportExports = (from p in reportvm
                                 group p by p.ProjectName into g
                                 select new ExportReports()
                                 {
                                     ProjectName = g.Key,
                                     reportVms = g.Select(c => c.report).ToList()
                                 }).ToList();

            var otherPj = reportExports.AsQueryable().Where(x => x.ProjectName == "Other").FirstOrDefault();
            if (otherPj != null)
            {
                reportExports.Remove(otherPj);
                reportExports.Add(otherPj);
            }

            //Ghi vào excel
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add((Month < 10 ? "0" : "") + Month.ToString() + "-" + Year.ToString());

                #region Fill template

                sheet.Cells["A1:J1"].Merge = true;
                sheet.Cells["A1:J1"].Value = "TIMESHEET";
                sheet.Cells["A1:J1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A1:J1"].Style.Font.Size = 20;
                sheet.Cells["A1:J1"].Style.Font.Bold = true;
                sheet.Row(1).Height = 25;

                sheet.Cells["E2:F2"].Merge = true;
                sheet.Cells["E2:F2"].Value = $"{Year} " + new DateTime(1999, Month, 12).ToString("MMM", CultureInfo.InvariantCulture);
                sheet.Cells["E2:F2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["E2:F2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["E2:F2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["E2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["E2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["E2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["E2:F2"].Style.Font.Size = 14;
                sheet.Cells["E2:F2"].Style.Font.Bold = true;
                sheet.Row(2).Height = 20;

                sheet.Cells["G4:I4"].Merge = true;
                sheet.Cells["G4:I4"].Value = "OT";
                sheet.Cells["G4:I4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["G4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["G4:I4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["G4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["G4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["B5:I5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["B5:I5"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                sheet.Cells["B5:I5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B5:I5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B5:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B5:I5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["B5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[5, 2].Value = "No";
                sheet.Cells[5, 3].Value = "Project Name";
                sheet.Cells[5, 4].Value = "Member";
                sheet.Cells[5, 5].Value = "Role";
                sheet.Cells[5, 6].Value = "Working Hour";
                sheet.Cells[5, 7].Value = "100%";
                sheet.Cells[5, 8].Value = "150%";
                sheet.Cells[5, 9].Value = "200%";

                #endregion

                int rowIndex = 6;
                int stt = 1;
                //export data từng project
                foreach (var result in reportExports)
                {
                    int startIndex = rowIndex;
                    sheet.Cells[rowIndex, 2].Value = stt;
                    sheet.Cells[rowIndex, 3].Value = result.ProjectName;
                    stt++;

                    //export data các member trong project
                    foreach (var rp in result.reportVms)
                    {
                        sheet.Cells[rowIndex, 4].Value = rp.DisplayName;
                        sheet.Cells[rowIndex, 5].Value = rp.Role;
                        sheet.Cells[rowIndex, 6].Value = rp.WorkingHour;
                        sheet.Cells[rowIndex, 7].Value = rp.Rate100;
                        sheet.Cells[rowIndex, 8].Value = rp.Rate150;
                        sheet.Cells[rowIndex, 9].Value = rp.Rate200;
                        rowIndex++;
                    }
                    //Mỗi project ít nhất 25 dòng (kế toán yêu cầu như vậy)
                    while (rowIndex - startIndex < 25)
                    {
                        rowIndex++;
                    }

                    sheet.Cells["B" + startIndex.ToString() + ":B" + (rowIndex - 1).ToString()].Merge = true;
                    sheet.Cells["C" + startIndex.ToString() + ":C" + (rowIndex - 1).ToString()].Merge = true;
                }
                var endrow = rowIndex;
                if (endrow > 6) //has at least 1 record
                {
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[$"B6:I{endrow - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                sheet.Columns[1, 10].AutoFit();
                sheet.Columns[3].Width = 30;
                package.Save();
            }

            stream.Position = 0;
            string fileName = Message.EXPORT_REPORT_NAME + DateTime.Now.ToString(Message.FORMAT_DATETIME) + Message.FILE_TYPE_EXCEL;

            return File(stream, Message.CONTENT_TYPE_FILE_EXPORT, fileName);
        }

        //xuất excel logtime theo từ member (trong tháng)
        [HttpGet("ExportReportTimeSheet/{time}/{isPartner}")]
        [Authorize]
        public async Task<IActionResult> ExportReportTimeSheet([FromRoute] string time, [FromRoute] bool isPartner)
        {
            string month = time.Substring(0, time.Length - 4);
            string years = time.Substring(time.Length - 4);
            var days = System.DateTime.DaysInMonth(int.Parse(years), int.Parse(month));
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var userProfile = (from u in _db.Users where u.Id == userid select u).FirstOrDefault();
            var roleName = await _userManager.GetRolesAsync(userProfile);

            #region get list Report
            //Lấy list những member có log time

            var listUserActiveInProject = (from userProject in _db.UserProjects
                                           join user in _db.Users on userProject.UserId equals user.Id
                                           where user.IsDeleted == false
                                                    && userProject.IsDeleted == false
                                                    && (user.UserInfo.Department == 2) == isPartner
                                           orderby user.DisplayName
                                           select new UserViewModel
                                           {
                                               Id = user.Id,
                                               DisplayName = user.DisplayName,
                                               Email = user.Email
                                           }).Distinct().ToList();

            //lấy list report
            var listReport = (from user in _db.Users
                              join report in _db.Reports on user.Id equals report.UserId
                              join userRole in _db.UserRoles on user.Id equals userRole.UserId
                              join role in _db.Roles on userRole.RoleId equals role.Id
                              where user.IsDeleted == false && report.IsDeleted == false && report.WorkingDay.Month == int.Parse(month)
                              && report.WorkingDay.Year == int.Parse(years)
                              orderby report.WorkingDay
                              select new ExportReportModel
                              {
                                  Id = user.Id,
                                  DisplayName = user.DisplayName,
                                  WorkingDay = report.WorkingDay,
                                  WorkingHour = report.WorkingHour,
                                  ReportType = report.ReportType,
                                  Status = report.Status,
                                  DayOnMonth = report.WorkingDay.Day,
                                  RateValue = report.RateValue,
                                  Email = user.Email,
                                  Department = user.UserInfo.Department,
                                  RoleName = role.Name,
                                  WorkingType = report.WorkingType,
                              }
                              ).ToList();

            //lấy list reportOff
            var listReportOffs = (from user in _db.Users
                                  join reportOffs in _db.ReportOffs on user.Id equals reportOffs.UserId
                                  where user.IsDeleted == false && reportOffs.IsDeleted == false && reportOffs.OffDateStart.Month == int.Parse(month)
                                  select new ExportReportOffsModel
                                  {
                                      Id = user.Id,
                                      DisplayName = user.DisplayName,
                                      OffDateStart = reportOffs.OffDateStart,
                                      OffDateEnd = reportOffs.OffDateEnd,
                                      Status = reportOffs.Status,
                                      OffTypeId = reportOffs.OffTypeId
                                  }).ToList();

            //lọc những report được duyệt
            var listReportApproved = listReport.Where(x => x.Status == Const.REPORT_STATUS_APPROVED).ToList();
            var listReportOffsApproved = listReportOffs.Where(x => x.Status == Const.REPORT_STATUS_APPROVED).ToList();
            if (listReportApproved.Count == 0 && listReportOffsApproved.Count == 0)
            {
                return Ok("No data to export!");
            }
            var listReportOffsNotApproved = listReportOffs.Where(x => x.Status != Const.REPORT_STATUS_APPROVED).ToList();
            #endregion

            //Ghi vào excel
            IWorkbook book = new XSSFWorkbook();
            ISheet sheet1 = book.CreateSheet(month + "-" + years);
            var boldFont = book.CreateFont();
            boldFont.FontHeightInPoints = 21;
            boldFont.IsBold = true;

            var boldFont2 = book.CreateFont();
            boldFont2.FontHeightInPoints = 12;
            boldFont2.IsBold = true;

            var headerRow = sheet1.CreateRow(0);

            #region style
            ICellStyle headerStyle = book.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.SetFont(boldFont);

            ICellStyle headerStyle2 = book.CreateCellStyle();
            headerStyle2.Alignment = HorizontalAlignment.Center;
            headerStyle2.VerticalAlignment = VerticalAlignment.Center;
            headerStyle2.SetFont(boldFont2);

            ICellStyle headerStyleTable = book.CreateCellStyle();
            headerStyleTable.FillForegroundColor = HSSFColor.LightOrange.Index;
            headerStyleTable.FillPattern = FillPattern.SolidForeground;
            headerStyleTable.Alignment = HorizontalAlignment.Right;
            headerStyleTable.VerticalAlignment = VerticalAlignment.Center;
            headerStyleTable.BorderBottom = BorderStyle.Medium;
            headerStyleTable.BorderLeft = BorderStyle.Medium;
            headerStyleTable.BorderRight = BorderStyle.Medium;
            headerStyleTable.BorderTop = BorderStyle.Medium;

            ICellStyle weekendStyleTable = book.CreateCellStyle();
            weekendStyleTable.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            weekendStyleTable.FillPattern = FillPattern.SolidForeground;
            weekendStyleTable.Alignment = HorizontalAlignment.Right;
            weekendStyleTable.VerticalAlignment = VerticalAlignment.Center;
            weekendStyleTable.BorderBottom = BorderStyle.Medium;
            weekendStyleTable.BorderLeft = BorderStyle.Medium;
            weekendStyleTable.BorderRight = BorderStyle.Medium;
            weekendStyleTable.BorderTop = BorderStyle.Medium;

            ICellStyle headerStyleTableL = book.CreateCellStyle();
            headerStyleTableL.FillForegroundColor = HSSFColor.LightOrange.Index;
            headerStyleTableL.FillPattern = FillPattern.SolidForeground;
            headerStyleTableL.Alignment = HorizontalAlignment.Left;
            headerStyleTableL.VerticalAlignment = VerticalAlignment.Center;
            headerStyleTableL.BorderBottom = BorderStyle.Medium;
            headerStyleTableL.BorderLeft = BorderStyle.Medium;
            headerStyleTableL.BorderRight = BorderStyle.Medium;
            headerStyleTableL.BorderTop = BorderStyle.Medium;

            ICellStyle dataStyleTable = book.CreateCellStyle();
            dataStyleTable.Alignment = HorizontalAlignment.Right;
            dataStyleTable.VerticalAlignment = VerticalAlignment.Center;
            dataStyleTable.BorderTop = BorderStyle.Thin;
            dataStyleTable.BorderLeft = BorderStyle.Thin;
            dataStyleTable.BorderRight = BorderStyle.Thin;
            dataStyleTable.BorderBottom = BorderStyle.Thin;

            ICellStyle onsiteCellStyle = book.CreateCellStyle();
            onsiteCellStyle.Alignment = HorizontalAlignment.Right;
            onsiteCellStyle.VerticalAlignment = VerticalAlignment.Center;
            onsiteCellStyle.BorderTop = BorderStyle.Thin;
            onsiteCellStyle.BorderLeft = BorderStyle.Thin;
            onsiteCellStyle.BorderRight = BorderStyle.Thin;
            onsiteCellStyle.BorderBottom = BorderStyle.Thin;
            onsiteCellStyle.FillForegroundColor = HSSFColor.LightOrange.Index;
            onsiteCellStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle offCellStyle = book.CreateCellStyle();
            offCellStyle.Alignment = HorizontalAlignment.Right;
            offCellStyle.VerticalAlignment = VerticalAlignment.Center;
            offCellStyle.BorderTop = BorderStyle.Thin;
            offCellStyle.BorderLeft = BorderStyle.Thin;
            offCellStyle.BorderRight = BorderStyle.Thin;
            offCellStyle.BorderBottom = BorderStyle.Thin;
            offCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            offCellStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle weekendDataStyleTable = book.CreateCellStyle();
            weekendDataStyleTable.Alignment = HorizontalAlignment.Right;
            weekendDataStyleTable.VerticalAlignment = VerticalAlignment.Center;
            weekendDataStyleTable.BorderTop = BorderStyle.Thin;
            weekendDataStyleTable.BorderLeft = BorderStyle.Thin;
            weekendDataStyleTable.BorderRight = BorderStyle.Thin;
            weekendDataStyleTable.BorderBottom = BorderStyle.Thin;
            weekendDataStyleTable.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            weekendDataStyleTable.FillPattern = FillPattern.SolidForeground;

            ICellStyle rawBorder = book.CreateCellStyle();
            rawBorder.BorderTop = BorderStyle.Thin;
            rawBorder.BorderLeft = BorderStyle.Thin;
            rawBorder.BorderRight = BorderStyle.Thin;
            rawBorder.BorderBottom = BorderStyle.Thin;

            ICellStyle r3Style = book.CreateCellStyle();
            r3Style.Alignment = HorizontalAlignment.Center;
            r3Style.VerticalAlignment = VerticalAlignment.Center;
            #endregion

            #region fill Header
            ICell r1c1 = headerRow.CreateCell(0);
            r1c1.SetCellValue("TIMESHEET");
            r1c1.CellStyle.WrapText = true;
            //r1c1.CellStyle.SetFont(IFont.Boldweight);
            r1c1.Row.Height = 800;
            CellRangeAddress cra = new CellRangeAddress(0, 0, 0, days + 11);
            r1c1.CellStyle = headerStyle;

            ICell r2cx = sheet1.CreateRow(1).CreateCell((days + 10) / 2 - 1);
            r2cx.SetCellValue(years + month);
            r2cx.CellStyle = headerStyle2;
            CellRangeAddress cra1 = new CellRangeAddress(1, 1, 0, days + 11);





            IRow rowNote = sheet1.CreateRow(3);
            ICell cellNote = rowNote.CreateCell(1);
            cellNote.CellStyle = onsiteCellStyle;
            cellNote = rowNote.CreateCell(2);
            cellNote.SetCellValue("Onsite");

            rowNote = sheet1.CreateRow(4);
            cellNote = rowNote.CreateCell(1);
            cellNote.CellStyle = dataStyleTable;
            cellNote = rowNote.CreateCell(2);
            cellNote.SetCellValue("Offline + Remote");

            rowNote = sheet1.CreateRow(5);
            cellNote = rowNote.CreateCell(1);
            cellNote.CellStyle = offCellStyle;
            cellNote = rowNote.CreateCell(2);
            cellNote.SetCellValue("Off work");
            //rowNote = sheet1.CreateRow(6);
            ICell r3cx = sheet1.CreateRow(6).CreateCell(days + 8);
            r3cx.SetCellValue("OT");
            r3cx.CellStyle = r3Style;

            CellRangeAddress cra2 = new CellRangeAddress(6, 6, days + 8, days + 10);

            sheet1.AddMergedRegion(cra);
            sheet1.AddMergedRegion(cra1);
            sheet1.AddMergedRegion(cra2);

            #endregion

            #region Sets the borders to the merged cell
            RegionUtil.SetBorderTop((int)BorderStyle.Thin, cra2, sheet1);
            RegionUtil.SetBorderLeft((int)BorderStyle.Thin, cra2, sheet1);
            RegionUtil.SetBorderRight((int)BorderStyle.Thin, cra2, sheet1);
            RegionUtil.SetBorderBottom((int)BorderStyle.Thin, cra2, sheet1);

            #endregion

            #region Header
            IRow headerRow01 = sheet1.CreateRow(7);
            ICell cell1 = headerRow01.CreateCell(1);
            cell1.SetCellValue("No");
            cell1.CellStyle = headerStyleTableL;
            ICell cell2 = headerRow01.CreateCell(2);
            cell2.SetCellValue("Full name");
            cell2.CellStyle = headerStyleTableL;
            ICell cell3 = headerRow01.CreateCell(3);
            cell3.SetCellValue("Email");
            cell3.CellStyle = headerStyleTableL;
            ICell cell4 = headerRow01.CreateCell(4);
            cell4.SetCellValue("Role");
            cell4.CellStyle = headerStyleTableL;
            ICell cell5 = headerRow01.CreateCell(5);
            cell5.SetCellValue("Division");
            cell5.CellStyle = headerStyleTableL;
            for (int i = 1; i <= days; i++)
            {
                ICell cell = headerRow01.CreateCell(i + 5);
                if (i < 10)
                    cell.SetCellValue("0" + i.ToString() + "/" + month + "/" + years);
                else
                    cell.SetCellValue(i.ToString() + "/" + month + "/" + years);
                cell.CellStyle = headerStyleTable;
                string mmddyyyy = month + "/" + i.ToString() + "/" + years;
                DayOfWeek getDate = Convert.ToDateTime(mmddyyyy).DayOfWeek;
                if (getDate == DayOfWeek.Saturday || getDate == DayOfWeek.Sunday)
                //if(Convert.ToDateTime(i.ToString() + "/" + month + "/" + years).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(i.ToString() + "/" + month + "/" + years).DayOfWeek == DayOfWeek.Sunday)
                {
                    cell.CellStyle = weekendStyleTable;
                }

            }
            ICell days6 = headerRow01.CreateCell(days + 6);
            days6.SetCellValue("Working hours");
            days6.CellStyle = headerStyleTableL;
            ICell days7 = headerRow01.CreateCell(days + 7);
            days7.SetCellValue("Offwork");
            days7.CellStyle = headerStyleTableL;
            ICell days8 = headerRow01.CreateCell(days + 8);
            days8.SetCellValue("100%");
            days8.CellStyle = headerStyleTable;
            ICell days9 = headerRow01.CreateCell(days + 9);
            days9.SetCellValue("150%");
            days9.CellStyle = headerStyleTable;
            ICell days10 = headerRow01.CreateCell(days + 10);
            days10.CellStyle = headerStyleTable;
            days10.SetCellValue("200%");
            #endregion

            #region Fill Data
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            var listUserReportApproved = listUserActiveInProject;
            int countUser = listUserReportApproved.Count();
            if (countUser > 0)
            {
                int i = 1;
                foreach (var item in listUserReportApproved)
                {
                    var userReportData = listReportApproved.Where(x => x.Id == item.Id).ToList();
                    var userReportOffData = listReportOffsApproved.Where(x => x.Id == item.Id).ToList();
                    var userReportOffDataReject = listReportOffsNotApproved.Where(x => x.Id == item.Id).ToList();
                    IRow row = sheet1.CreateRow(i + 7);
                    ICell cellSTT = row.CreateCell(1);
                    cellSTT.SetCellValue(i.ToString());
                    cellSTT.CellStyle = dataStyleTable;

                    ICell cellName = row.CreateCell(2);
                    cellName.SetCellValue(item.DisplayName);
                    cellName.CellStyle = rawBorder;
                    ICell cellEmail = row.CreateCell(3);
                    cellEmail.SetCellValue(item.Email);
                    cellEmail.CellStyle = rawBorder;
                    ICell cellRoleName = row.CreateCell(4);
                    cellRoleName.SetCellValue(userReportData.Select(x => x.RoleName).FirstOrDefault());
                    cellRoleName.CellStyle = rawBorder;
                    string departmentName = "";
                    switch (userReportData.Select(x => x.Department).FirstOrDefault())
                    {
                        case 0:
                            departmentName = "Division 1";
                            break;
                        case 1:
                            departmentName = "Division Faster";
                            break;
                        case 2:
                            departmentName = "Partner";
                            break;
                        default: break;
                    }
                    ICell cellDepartment = row.CreateCell(5);
                    cellDepartment.SetCellValue(departmentName);
                    cellDepartment.CellStyle = rawBorder;
                    float sum = 0;
                    for (int j = 1; j <= days; j++)
                    {
                        ICell cell = row.CreateCell(j + 5);
                        sum = userReportData.Where(c => c.DayOnMonth == j).Sum(x => x.WorkingHour);
                        cell.SetCellValue(sum.ToString());


                        string mmddyyyy = month + "/" + j.ToString() + "/" + years;
                        DayOfWeek getDate = Convert.ToDateTime(mmddyyyy).DayOfWeek;
                        if (getDate == DayOfWeek.Saturday || getDate == DayOfWeek.Sunday)
                        //if(Convert.ToDateTime(i.ToString() + "/" + month + "/" + years).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(i.ToString() + "/" + month + "/" + years).DayOfWeek == DayOfWeek.Sunday)
                        {
                            cell.CellStyle = weekendDataStyleTable;
                        }
                        if (sum == 0) cell.CellStyle = offCellStyle;
                        var workingTypeArr = userReportData.Where(c => c.DayOnMonth == j).Select(c => c.WorkingType).ToArray();
                        int workingType = -1;
                        if (workingTypeArr.Length == 0) cell.CellStyle = offCellStyle;
                        else workingType = workingTypeArr.Max();
                        switch (workingType)
                        {
                            case 3:
                                cell.CellStyle = onsiteCellStyle;
                                break;
                            case 2:
                                cell.CellStyle = dataStyleTable;
                                break;
                            case 0:
                                cell.CellStyle = dataStyleTable;
                                break;
                        }

                    }
                    double sumWorkingDays = userReportData.Where(x => x.ReportType == Const.REPORT_TYPE_NORMAL).Sum(x => x.WorkingHour);
                    ICell cellWorkingHours = row.CreateCell(days + 6);
                    sumWorkingDays = Math.Round(sumWorkingDays, 2);
                    cellWorkingHours.SetCellValue(sumWorkingDays.ToString());
                    cellWorkingHours.CellStyle = dataStyleTable;

                    double sumOffWorks = 0;
                    var paymentList = userReportOffData.Where(x => x.OffTypeId == 0);
                    foreach (var timeBreak in paymentList)
                    {
                        if (timeBreak.OffDateStart.Day == timeBreak.OffDateEnd.Day)
                        {
                            if (myCal.GetDayOfWeek(timeBreak.OffDateStart) != DayOfWeek.Sunday && myCal.GetDayOfWeek(timeBreak.OffDateStart) != DayOfWeek.Saturday)
                            {
                                sumOffWorks += new ReportHandler().CalculationTime(timeBreak.OffDateStart.TimeOfDay, timeBreak.OffDateEnd.TimeOfDay);
                            }
                        }
                        else
                        {

                            if (timeBreak.OffDateEnd.Month != int.Parse(month) && timeBreak.OffDateEnd.Year != int.Parse(years))
                            {
                                timeBreak.OffDateEnd = new DateTime(int.Parse(years), int.Parse(month), days, Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                            }
                            foreach (DateTime day in new ReportHandler().EachDay(timeBreak.OffDateStart, timeBreak.OffDateEnd))
                            {

                                if (myCal.GetDayOfWeek(day) != DayOfWeek.Sunday && myCal.GetDayOfWeek(day) != DayOfWeek.Saturday)
                                {
                                    var timeCheckIn = new TimeSpan();
                                    var timeCheckOut = new TimeSpan();
                                    if (day.Day == timeBreak.OffDateStart.Day)
                                    {
                                        timeCheckIn = timeBreak.OffDateStart.TimeOfDay;
                                        timeCheckOut = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                                    }
                                    else
                                    {
                                        if (day.Day == timeBreak.OffDateEnd.Day)
                                        {
                                            timeCheckIn = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                                            timeCheckOut = timeBreak.OffDateEnd.TimeOfDay;
                                        }
                                        else
                                        {
                                            timeCheckIn = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
                                            timeCheckOut = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
                                        }
                                    }
                                    sumOffWorks += new ReportHandler().CalculationTime(timeCheckIn, timeCheckOut);
                                }
                            }
                        }

                    }

                    ICell cellOffwork = row.CreateCell(days + 7);
                    sumOffWorks = Math.Round(sumOffWorks, 2);
                    cellOffwork.SetCellValue(sumOffWorks.ToString());
                    cellOffwork.CellStyle = dataStyleTable;

                    double OT100 = userReportData.Where(x => x.ReportType == Const.REPORT_TYPE_OT && x.RateValue == 100).Sum(x => x.WorkingHour);
                    double OT150 = userReportData.Where(x => x.ReportType == Const.REPORT_TYPE_OT && x.RateValue == 150).Sum(x => x.WorkingHour);
                    double OT200 = userReportData.Where(x => x.ReportType == Const.REPORT_TYPE_OT && x.RateValue == 200).Sum(x => x.WorkingHour);
                    ICell cellOT100 = row.CreateCell(days + 8);
                    cellOT100.SetCellValue(OT100.ToString());
                    cellOT100.CellStyle = dataStyleTable;
                    ICell cellOT150 = row.CreateCell(days + 9);
                    cellOT150.SetCellValue(OT150.ToString());
                    cellOT150.CellStyle = dataStyleTable;
                    ICell cellOT200 = row.CreateCell(days + 10);
                    cellOT200.SetCellValue(OT200.ToString());
                    cellOT200.CellStyle = dataStyleTable;
                    i++;
                }
                for (int col = 1; col <= days + 11; col++)
                {
                    sheet1.AutoSizeColumn(col);
                    sheet1.SetColumnWidth(col, sheet1.GetColumnWidth(col) + 1 * 256);
                }
            }



            #endregion

            string fileName = Message.EXPORT_REPORT_NAME + DateTime.Now.ToString(Message.FORMAT_DATETIME) + Message.FILE_TYPE_EXCEL;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Seek(0, SeekOrigin.Begin);
                book.Write(ms);

                var content = ms.ToArray();

                return File(content, Message.CONTENT_TYPE_FILE_EXPORT, fileName);
            }

        }

        //Lấy thông tin dự án cho báo cáo
        [HttpGet("GetProjectForReport")]
        [Authorize]
        public async Task<List<ProjectForReportViewModel>> GetProjectForReport()
        {

            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var ur = await _userManager.FindByIdAsync(userid);
            var role = await _userManager.GetRolesAsync(ur);

            var project = (from p in _db.Projects
                           join up in _db.UserProjects on p.ProjectId equals up.ProjectId
                           where up.UserId == userid && up.PositionId == Const.POSITION_PROJECT_PM
                           select new ProjectForReportViewModel
                           {
                               ProjectId = p.ProjectId,
                               Name = p.ProjectName
                           }).Distinct().ToList();

            var projectList = await _db.Projects.Select(x => new ProjectForReportViewModel()
            {
                ProjectId = x.ProjectId,
                Name = x.ProjectName
            }).ToListAsync();

            if (project.Count() > 0 && role.FirstOrDefault() != BSRole.MANAGER && role.FirstOrDefault() != BSRole.SYSADMIN)
            {
                int[] projectId = project.Select(x => (int)x.ProjectId).ToArray();

                projectList = projectList.Where(x => projectId.Contains(x.ProjectId)).ToList();
            }


            return projectList;
        }

        //Tìm danh sách member trong dự án có id xác định
        [HttpGet("GetUserInProject/{id}/{name}")]
        [Authorize]
        public async Task<List<GetUserForReport>> GetUserInProject(int id, string name)
        {
            //Lấy toàn bộ list member
            var users = await (from up in _db.UserProjects
                               where up.ProjectId == id
                               join ur in _db.Users on up.UserId equals ur.Id
                               select new GetUserForReport
                               {
                                   UserId = ur.Id,
                                   DisplayName = ur.DisplayName
                               }).Distinct().ToListAsync();

            //Lọc member theo search key
            if (name != "all_User_Off_Project")
            {
                var result = new List<GetUserForReport>();
                foreach (GetUserForReport user in users)
                {
                    if (user.DisplayName.ToLower().Contains(name.ToLower()))
                        result.Add(user);
                }
                return result;
            }
            else
                return users;
        }

        //Lấy các position để log vào report
        [HttpGet]
        [Route("GetPositionsForReport")]
        [Authorize]
        public async Task<List<PositionViewModel>> GetPosition()
        {
            var positions = await _db.Positions.Where(x => x.PositionName != BSRole.MEMBER && x.PositionName != BSRole.PRODUCT_MANAGER).Select(p => new PositionViewModel
            {
                PositionId = p.PositionId,
                PositionName = p.PositionName
            }).ToListAsync();

            return positions;
        }

        //Lấy list những type nghỉ (có lương, k lương, đặc biệt khác vvv)
        [HttpGet("GetOffType")]
        [Authorize]
        public async Task<List<OffTypeVm>> GetOffType()
        {
            var result = await _db.OffTypes.Select(t => new OffTypeVm
            {
                Id = t.Id,
                Name = t.Name
            }).OrderBy(t => t.Id).ToListAsync();

            return result;
        }

        [HttpGet("ExportPendingMemberList/{time}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<IActionResult> ExportPendingMemberList([FromRoute] string time)
        {
            string month = time.Substring(0, time.Length - 4);
            string years = time.Substring(time.Length - 4);

            var lst = await (from u in _db.Users
                             where u.UserInfo.IsPending && !u.IsDeleted
                             select new
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

            var res = (from u in lst
                       group u by u.Team into gt
                       select new
                       {
                           Team = gt.Key,
                           Levels = (from l in lst
                                     where l.Team == gt.Key
                                     group l by l.Level into gl
                                     select new
                                     {
                                         Name = gl.Key,
                                         Members = gl.Select(m => new
                                         {
                                             DisplayName = m.DisplayName,
                                             Id = m.Id,
                                             Email = m.Email,
                                             PendingStart = m.PendingStart,
                                             EffortFree = m.EffortFree,
                                             CVLink = m.CVLink,
                                         })
                                     })
                       });


            //Ghi vào excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add(month + "-" + years);

                #region Fill template
                sheet.Cells["A1:G1"].Merge = true;
                sheet.Cells["A1:G1"].Value = "Tổng số member đang chờ việc: " + lst.Count.ToString();
                sheet.Cells["A1:G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A1:G1"].Style.Font.Size = 20;
                sheet.Cells["A1:G1"].Style.Font.Bold = false;
                sheet.Row(1).Height = 25;

                sheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                sheet.Cells["A4:G4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A4:G4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A4:G4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A4:G4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[4, 1].Value = "No";
                sheet.Cells[4, 2].Value = "Team";
                sheet.Cells[4, 3].Value = "Level";
                sheet.Cells[4, 4].Value = "Member's name";
                sheet.Cells[4, 5].Value = "Pending from";
                sheet.Cells[4, 6].Value = "Effort free";
                sheet.Cells[4, 7].Value = "CV link";

                int rowIndex = 5;
                int stt = 1;

                foreach (var item in res)
                {
                    int numbersOfTeamMember = 0;
                    int startTeamIndex = rowIndex;
                    sheet.Cells[rowIndex, 1].Value = stt;

                    stt++;

                    foreach (var level in item.Levels)
                    {
                        int numbersOfLevelMember = 0;
                        int startLevelIndex = rowIndex;
                        //sheet.Cells[rowIndex, 4].Value = level.Name;

                        foreach (var member in level.Members)
                        {
                            sheet.Cells[rowIndex, 4].Value = member.DisplayName;
                            sheet.Cells[rowIndex, 5].Value = member.PendingStart?.ToString(Message.FORMAT_DATE);
                            sheet.Cells[rowIndex, 6].Value = member.EffortFree.ToString() + "%";
                            sheet.Cells[rowIndex, 7].Value = member.CVLink;
                            rowIndex++;
                            numbersOfLevelMember++;
                            numbersOfTeamMember++;
                        }

                        sheet.Cells[startLevelIndex, 3].Value = level.Name + " (" + numbersOfLevelMember.ToString() + ")";
                        sheet.Cells["C" + startLevelIndex.ToString() + ":C" + (rowIndex - 1).ToString()].Merge = true;
                    }

                    sheet.Cells[startTeamIndex, 2].Value = item.Team + " (" + numbersOfTeamMember.ToString() + ")";
                    sheet.Cells["A" + startTeamIndex.ToString() + ":A" + (rowIndex - 1).ToString()].Merge = true;
                    sheet.Cells["B" + startTeamIndex.ToString() + ":B" + (rowIndex - 1).ToString()].Merge = true;
                }

                if (rowIndex > 5) //has at least 1 record
                {
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A5:G{rowIndex - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                sheet.Columns[1, 10].AutoFit();
                sheet.Columns[2].Width = 30;
                sheet.Columns[3].Width = 20;
                package.Save();

                stream.Position = 0;
                string fileName = "Report_PendingMember_" + DateTime.Now.ToString(Message.FORMAT_DATETIME) + Message.FILE_TYPE_EXCEL;

                return File(stream, Message.CONTENT_TYPE_FILE_EXPORT, fileName);

                #endregion
            }

        }

        //Tính toán thời gian nghỉ
        private double CalculateOffTme(TimeSpan timeOfDayStart, TimeSpan timeOfDayEnd)
        {
            TimeSpan inTime = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
            TimeSpan outTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
            TimeSpan breakTimeStart = new TimeSpan(12, 0, 0);
            TimeSpan breakTimeEnd = new TimeSpan(13, 0, 0);
            TimeSpan workTime = new TimeSpan();
            double totalTime = 0;

            if (timeOfDayEnd > timeOfDayStart)
            {
                TimeSpan checkIn = new TimeSpan(0, 0, 0);
                TimeSpan checkOut = new TimeSpan(0, 0, 0);
                TimeSpan breakTime = new TimeSpan(0, 0, 0);
                if (timeOfDayStart <= inTime) //<=8h
                {
                    checkIn = inTime;
                }
                else// >8h
                {
                    if (timeOfDayStart <= breakTimeStart)// <=12h
                    {
                        checkIn = timeOfDayStart;
                    }
                    else // >12h
                    {
                        if (timeOfDayStart <= breakTimeEnd) // <=13h
                        {
                            checkIn = breakTimeEnd;
                        }
                        else //>13h
                        {
                            if (timeOfDayStart <= outTime) //<=17h
                            {
                                checkIn = timeOfDayStart;
                            }
                            else // >17h
                            {
                                checkIn = outTime;
                            }
                        }
                    }
                }

                if (timeOfDayEnd >= checkOut)//>=17h
                {
                    checkOut = outTime;
                }//<17h
                {
                    if (timeOfDayEnd >= breakTimeEnd)//>=13h
                    {
                        checkOut = timeOfDayEnd;
                    }
                    else //<13h
                    {
                        if (timeOfDayEnd >= breakTimeStart)//>=12h
                        {
                            checkOut = breakTimeStart;
                        }    //<12h
                        else
                        {
                            if (timeOfDayEnd >= inTime)//>=8h
                            {
                                checkOut = timeOfDayEnd;
                            }
                            else//<8h
                            {
                                checkOut = inTime;
                            }
                        }
                    }
                }
                //
                if (timeOfDayStart <= breakTimeStart && timeOfDayEnd >= breakTimeEnd)
                {
                    breakTime = new TimeSpan(1, 0, 0);
                }

                workTime = checkOut - checkIn - breakTime;
                totalTime = workTime.TotalHours;
            }

            return totalTime;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from; day.Date <= to.Date; day = day.AddDays(1))
            {
                if (day.Date == to.Date)
                    yield return to;
                else
                    yield return day;
            }
        }

    }//end class

}//end namespace