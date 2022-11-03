using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.ViewModels;
using System.Linq;
using UsageHelper;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingController : ControllerBaseBS
    {
        private readonly IMemoryCache cache;

        public SettingController(UsageDbContext context, IMemoryCache cache) : base(context)
        {
            this.cache = cache;
        }


        [HttpPost]
        [Route("CreatePosition")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<ActionResult> CreatePosition([FromBody] string name)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var position = new BS_Position();
            position.PositionName = name;

            _db.Add(position);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("CreateFramework")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<ActionResult> CreateFramework([FromBody] string name)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var framework = new BS_Framework();
            framework.FrameworkName = name;

            _db.Add(framework);
            await _db.SaveChangesAsync();

            return Ok();
        }


        // Lấy danh sách các vị trí 
        [HttpGet]
        [Route("GetPositions")]
        public async Task<List<PositionViewModel>> GetPositions()
        {
            if (!cache.TryGetValue(Const.KEY_POSITION, out List<PositionViewModel> positions))
            {
                //Positions not found in cache. set the cache Positions here
                positions = await _db.Positions.Select(x => new PositionViewModel()
                {
                    PositionId = x.PositionId,
                    PositionName = x.PositionName
                }).ToListAsync();

                cache.Set(Const.KEY_POSITION, positions);
            }

            return positions;
        }

        //Lấy list các framework
        [HttpGet("GetFrameworks")]
        public async Task<List<FrameViewModel>> GetFrameworks()
        {
            if (!cache.TryGetValue(Const.KEY_FRAMEWORK, out List<FrameViewModel> frameWorks))
            {
                //Frameworks not found in cache. set the cache Frameworks here
                frameWorks = await _db.Frameworks.Select(x => new FrameViewModel()
                {
                    Id = x.FrameworkId,
                    FrameworkName = x.FrameworkName
                }).ToListAsync();

                cache.Set(Const.KEY_FRAMEWORK, frameWorks);
            }

            return frameWorks;
        }

        //Lấy list các language
        [HttpGet("GetLanguages")]
        public async Task<List<LanguageViewModel>> GetLanguages()
        {
            if (!cache.TryGetValue(Const.KEY_LANGUAGE, out List<LanguageViewModel> languages))
            {
                //Languages not found in cache. set the cache Languages here
                languages = await _db.Languages.Select(x => new LanguageViewModel()
                {
                    Id = x.LanguageId,
                    LanguageName = x.LanguageName
                }).ToListAsync();

                cache.Set(Const.KEY_LANGUAGE, languages);
            }

            return languages;
        }

        //Lấy list các level
        [HttpGet("GetLevels")]
        public async Task<List<LevelViewModel>> GetLevels()
        {
            if (!cache.TryGetValue(Const.KEY_LEVEL, out List<LevelViewModel> levels))
            {
                //Levels not found in cache. set the cache Levels here
                levels = await _db.Levels.Select(x => new LevelViewModel()
                {
                    Id = x.LevelId,
                    LevelName = x.LevelName
                }).ToListAsync();

                cache.Set(Const.KEY_LEVEL, levels);
            }
            return levels;
        }



        //Tạo thiết lập lương mới
        [HttpPost]
        [Route("CreateSalary")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<ActionResult> CreateSalary([FromBody] SalaryDTO request)
        {
            foreach (var userId in request.User_Id)
            {
                var salary = new BS_UserSalaries();

                salary.UserId = userId;
                salary.HourlySalary = request.HourlySalary;
                salary.EffectiveDate = request.EffectiveDate;

                _db.Add(salary);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Cập nhật lương member
        [HttpPut("UpdateSalary")]
        [Authorize]
        public async Task<IActionResult> UpdateSalary([FromBody] SalaryDTO request)
        {
            var salary = await _db.UserSalaries.FirstOrDefaultAsync(x => x.Id == request.Id);

            //thông tin cơ bản
            salary.HourlySalary = request.HourlySalary;
            salary.EffectiveDate = request.EffectiveDate;

            _db.Update(salary);

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy list lương member
        [HttpGet("GetSalaries")]
        [Authorize]
        public async Task<List<SalaryViewModel>> GetSalaries([FromQuery] SalaryQueryParameters queryParameters)
        {
            var queryable = _db.UserSalaries.Join(_db.Users, ur => ur.UserId, u => u.Id, (ur, u) => new
            {
                ur = ur,
                u = u
            }).Select(x => new { x.ur, x.u });

            HttpContext.InsertParametersPaginationInHeader(queryable);

            var data = await queryable.Paginage(queryParameters.PaginationVm).Select(x => new SalaryViewModel()
            {
                User_Id = x.ur.UserId,
                DisplayName = x.u.DisplayName,
                Email = x.u.Email,
                HourlySalary = x.ur.HourlySalary,
                EffectiveDate = x.ur.EffectiveDate,
                IsDeleted = x.ur.IsDeleted,
                Id = x.ur.Id,
            }).ToListAsync();

            return data;
        }

        //Lấy thông tin lương member có id xác định
        [HttpGet("GetSalary/{id}")]
        [Authorize]
        public async Task<SalaryViewModel> GetSalary(int id)
        {
            var salary = await _db.UserSalaries.FirstOrDefaultAsync(x => x.Id == id);

            var salaryVM = new SalaryViewModel()
            {
                User_Id = salary.UserId,
                HourlySalary = salary.HourlySalary,
                EffectiveDate = salary.EffectiveDate,
                IsDeleted = salary.IsDeleted,
                Id = salary.Id
            };

            return salaryVM;
        }

        //Khoá - mở khoá lương member
        [HttpPatch("ChangeSalaryStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> ChangeSalaryStatus([FromRoute] int id, [FromBody] bool isDeleted)
        {
            var salary = await _db.UserSalaries.FirstOrDefaultAsync(x => x.Id == id);

            if (salary == null)
                return NotFound();

            salary.IsDeleted = isDeleted;

            _db.Update(salary);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("GetAllFramework")]
        public async Task<List<FrameViewModel>> GetAllFramework()
        {
            var data = await _db.Frameworks.Select(x => new FrameViewModel()
            {
                Id = x.FrameworkId,
                FrameworkName = x.FrameworkName
            }).ToListAsync();

            return data;
        }

        //Lấy list các language
        [HttpGet("GetAllLanguage")]
        public async Task<List<LanguageViewModel>> GetAllLanguage()
        {
            var data = await _db.Languages.Select(x => new LanguageViewModel()
            {
                Id = x.LanguageId,
                LanguageName = x.LanguageName
            }).ToListAsync();

            return data;
        }

        //Lấy list các level
        [HttpGet("GetAllLevel")]
        public async Task<List<LevelViewModel>> GetAllLevel()
        {
            var data = await _db.Levels.Select(x => new LevelViewModel()
            {
                Id = x.LevelId,
                LevelName = x.LevelName
            }).ToListAsync();

            return data;
        }
    }
}
