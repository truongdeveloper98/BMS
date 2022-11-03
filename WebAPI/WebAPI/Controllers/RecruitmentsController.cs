using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class RecruitmentsController : ControllerBaseBS
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruitmentsController(UsageDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        //Lấy list các tin tuyển dụng
        [HttpGet("GetAll")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<List<RecruitmentViewModel>> GetAll([FromQuery] RecruitmentQueryParameters queryParameters)
        {
            var search = queryParameters.Search ?? "";

            var recruit = _db.Recruitments.Join(_db.Levels, re => re.LevelId, le => le.LevelId, (recruitment, level) => new
            {
                r = recruitment,
                l = level

            }).Join(_db.Languages, re => re.r.LanguageId, lg => lg.LanguageId, (recruitmentLg, lg) => new
            {
                rlg = recruitmentLg,
                lg = lg

            }).Join(_db.Frameworks, rf => rf.rlg.r.LanguageId, f => f.FrameworkId, (recruitmentFr, f) => new
            {
                rf = recruitmentFr,
                f = f
            }).Join(_db.Positions, rp => rp.rf.rlg.r.PositionId, p => p.PositionId, (recruitmentPos, pos) => new
            {
                rp = recruitmentPos,
                pos = pos
            }).Select(c => new
            {
                c.rp.rf.rlg.r,
                c.pos,
                c.rp.f,
                c.rp.rf.lg,
                c.rp.rf.rlg.l

            });

            recruit = recruit.Where(x => x.r.Description.Contains(search));

            if (queryParameters.Status != null)
            {
                recruit = recruit.Where(x => x.r.Status == queryParameters.Status);
            }

            if (queryParameters.Result != null)
            {
                recruit = recruit.Where(x => x.r.Result == queryParameters.Result);
            }

            if (queryParameters.Position != null)
            {
                recruit = recruit.Where(x => x.pos.PositionId == queryParameters.Position);

            }

            HttpContext.InsertParametersPaginationInHeader(recruit);

            var data = new List<RecruitmentViewModel>();

            data = await recruit.Paginage(queryParameters.PaginationVm).Select(x => new RecruitmentViewModel()
            {
                Id = x.r.Id,
                DateOnBroad = (DateTime)Usage.Date2Utc(x.r.DateOnBroad),
                DatePublish = (DateTime)Usage.Date2Utc(x.r.DatePublish),
                Description = x.r.Description,
                Status = x.r.Status,
                Result = x.r.Result,
                Priority = x.r.Priority,
                LanguageName = x.lg.LanguageName,
                PositionName = x.pos.PositionName,
                FrameworkName = x.f.FrameworkName,
                Quantity = x.r.Quantity,
                SalaryMin = x.r.SalaryMin,
                SalaryMax = x.r.SalaryMax,
            }).OrderBy(x => x.Status).Distinct().ToListAsync();

            return data;
        }

        //Tạo tin tuyển dụng mới
        [HttpPost]
        [Route("CreateRecuitment")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<ActionResult> CreateRecuitment([FromBody] RecruitmentViewModel request)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var recruitment = new BS_Recruitment();
            recruitment.Created_By = userid;
            recruitment.SalaryMin = request.SalaryMin;
            recruitment.SalaryMax = request.SalaryMax;
            recruitment.Quantity = request.Quantity;
            recruitment.Description = request.Description;
            recruitment.DatePublish = (DateTime)Usage.Date2Utc(request.DatePublish);
            recruitment.DateOnBroad = (DateTime)Usage.Date2Utc(request.DateOnBroad);
            recruitment.LanguageId = request.LanguageId;
            recruitment.LevelId = request.LevelId;
            recruitment.PositionId = request.PositionId;
            recruitment.FrameworkId = request.FrameworkId;
            recruitment.Status = 0;
            recruitment.Result = 0;
            recruitment.Priority = 0;

            _db.Add(recruitment);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy thông tin tuyển dụng có id xác định
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<RecruitmentViewModel> GetById(int id)
        {
            var recruit = _db.Recruitments.Join(_db.Levels, re => re.LevelId, le => le.LevelId, (recruitment, level) => new
            {
                r = recruitment,
                l = level

            }).Join(_db.Languages, re => re.r.LanguageId, lg => lg.LanguageId, (recruitmentLg, lg) => new
            {
                rlg = recruitmentLg,
                lg = lg

            }).Join(_db.Frameworks, rf => rf.rlg.r.LanguageId, f => f.FrameworkId, (recruitmentFr, f) => new
            {
                rf = recruitmentFr,
                f = f
            }).Join(_db.Positions, rp => rp.rf.rlg.r.PositionId, p => p.PositionId, (recruitmentPos, pos) => new
            {
                rp = recruitmentPos,
                pos = pos
            }).Where(x=> x.rp.rf.rlg.r.Id == id).Select(c => new
            {
                c.rp.rf.rlg.r,
                c.pos,
                c.rp.f,
                c.rp.rf.lg,
                c.rp.rf.rlg.l

            });

            var recruitmentVm = await recruit.Select(x => new RecruitmentViewModel()
            {
                SalaryMin = x.r.SalaryMin,
                SalaryMax = x.r.SalaryMax,
                Quantity = x.r.Quantity,
                DateOnBroad = (DateTime)Usage.Date2Utc(x.r.DateOnBroad),
                DatePublish = (DateTime)Usage.Date2Utc(x.r.DatePublish),
                Description = x.r.Description,
                LanguageId = x.lg.LanguageId,
                LevelId = x.l.LevelId,
                PositionId = x.pos.PositionId,
                FrameworkId = x.f.FrameworkId
            }).FirstOrDefaultAsync();

            return recruitmentVm;
        }

        //Cập nhật thông tin tuyển dụng
        [HttpPut("UpdateReCruitment")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<IActionResult> UpdateReCruitment([FromBody] RecruitmentViewModel request)
        {
            var recruitment = await _db.Recruitments.FirstOrDefaultAsync(x => x.Id == request.Id);

            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            //thông tin cơ bản của tin tuyển dụng
            recruitment.UserId = userid;
            recruitment.SalaryMin = request.SalaryMin;
            recruitment.SalaryMax = request.SalaryMax;
            recruitment.Quantity = request.Quantity;
            recruitment.Description = request.Description;
            recruitment.DatePublish = (DateTime)Usage.Date2Utc(request.DatePublish);
            recruitment.DateOnBroad = (DateTime)Usage.Date2Utc(request.DateOnBroad);
            recruitment.LanguageId = request.LanguageId;
            recruitment.LevelId = request.LevelId;
            recruitment.PositionId = request.PositionId;
            recruitment.FrameworkId = request.FrameworkId;
            _db.Update(recruitment);

            //cập nhập các framework trong tin tuyển dụng

            await _db.SaveChangesAsync();

            return Ok();
        }

        //change status tin tuyển dụng
        [HttpPatch("ChangeStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] int status)
        {
            var project = await _db.Recruitments.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
                return NotFound();

            project.Result = status;

            _db.Update(project);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //change Priority tin tuyển dụng
        [HttpPatch("ChangePriority/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<IActionResult> ChangePriority([FromRoute] int id, [FromBody] int status)
        {
            var project = await _db.Recruitments.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null) 
                return NotFound();

            project.Priority = status;

            _db.Update(project);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //change result tin tuyển dụng
        [HttpPatch("ChangeResult/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<IActionResult> ChangeResult([FromRoute] int id, [FromBody] int status)
        {
            var project = await _db.Recruitments.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null) 
                return NotFound();

            project.Status = status;

            _db.Update(project);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("DashboardStat/{time}")]
        [Authorize]
        public async Task<object> GetRecruimentDashboardStat([FromRoute] string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));
            DateTime dateRef = new DateTime(year, month, 1);
            var day = System.DateTime.DaysInMonth(year, month);
            DateTime endDateRef = new DateTime(year, month, day);

            var recruimentList = await _db.Recruitments.Where(r => r.DatePublish < endDateRef).Select(x => new
            {
                Status = x.Status,
                RecruitmentId = x.Id,
            }).Distinct().ToListAsync();

            var res = new
            {
                Total = recruimentList.Count,
                Chartdata = recruimentList.GroupBy(x => x.Status).Select(i => new
                {
                    Status = i.Key,
                    Count = i.Count()
                })
            };

            return res;
        }

        [HttpGet("DashboardStatPosition/{time}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.SALE + "," + BSRole.HR)]
        public async Task<object> DashboardStatPosition([FromRoute] string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));
            DateTime dateRef = new DateTime(year, month, 1);
            var day = System.DateTime.DaysInMonth(year, month);
            DateTime endDateRef = new DateTime(year, month, day);

            var recruimentList = await _db.Recruitments.Join(_db.Positions, r => r.PositionId, p => p.PositionId, (r, p) => new
            {
                r = r,
                p = p
            }).Where(x => x.r.DatePublish < endDateRef).Select(i => new
            {
                Position = i.p.PositionName,
                RecruitmentId = i.r.Id,
            }).Distinct().ToListAsync();

            var res = new
            {
                Total = recruimentList.Count,
                Chartdata = recruimentList.GroupBy(x => x.Position).Select(i => new
                {
                    Position = i.Key,
                    Count = i.Count()
                })
            };

            return res;
        }
    }
}
