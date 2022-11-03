//Controller chứa function quản lý các dự án

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.Models;
using WebAPI.Models.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBaseBS
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(UsageDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        //Lấy danh sách các vị trí có trong dự án
        [HttpGet]
        [Route("GetPositions")]
        [Authorize]
        public async Task<List<PositionViewModel>> GetPosition()
        {
            return await _db.Positions.Where(p => !p.IsDeleted).Select(p => new PositionViewModel
            {
                PositionId = p.PositionId,
                PositionName = p.PositionName
            }).ToListAsync();
        }

        //Lấy list các type dự án
        [HttpGet]
        [Route("GetProjectTypes")]
        [Authorize]
        public async Task<List<ProjectTypeViewModel>> GetProjectType()
        {
            return await _db.ProjectTypes.Select(p => new ProjectTypeViewModel
            {
                ProjectTypeId = p.ProjectTypeId,
                ProjectTypeName = p.ProjectType_Name
            }).ToListAsync();
        }

        //Tạo dự án mới
        [HttpPost]
        [Route("CreateProject")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<ActionResult> CreateProject([FromBody] ProjectViewModel request)
        {
            var projectType = _db.ProjectTypes.FirstOrDefault(x => x.ProjectTypeId == request.ProjectTypeId);

            if (projectType == null) 
                return NotFound();

            var project = new BS_Project();
            project.ProjectName = request.Project_Name;
            project.ProjectTypeId = request.ProjectTypeId;
            project.PartnerId = request.PartnerId;
            project.CustomerId = request.CustomerId;
            project.Code = request.Project_Code;
            project.Revenue = request.Revenua;
            project.PM_Estimate = request.PM_Estimate ?? 0;
            project.Tester_Estimate = request.Tester_Estimate ?? 0;
            project.Brse_Estimate = request.Brse_Estimate ?? 0;
            project.Comtor_Estimate = request.Comtor_Estimate ?? 0;
            project.Developer_Estimate = request.Developer_Estimate ?? 0;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.BackLogLink = request.BacklogLink;
            project.Status = Const.REPORT_STATUS_WAITING;

            _db.Add(project);
            await _db.SaveChangesAsync();

            if (request.UserPositions.Count() > 0)
            {
                CreateUserProjects(project.ProjectId, request.UserPositions);
                await _db.SaveChangesAsync();
            }

            return Ok();
        }

        //Cập nhật thông tin dự án
        [HttpPut("UpdateProject")]
        [Authorize]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectViewModel model)
        {
            // Only pm,admin,manager have a permission to update project
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var ur = await _userManager.FindByIdAsync(userid);
            var role = await _userManager.GetRolesAsync(ur);
            var isPm = (await _db.UserProjects.Where(x => x.UserId == userid && x.PositionId == Const.POSITION_PROJECT_PM).CountAsync())>0?true:false;
            var isUpdated=false;
            if (role.FirstOrDefault() == BSRole.SYSADMIN || role.FirstOrDefault() == BSRole.MANAGER || isPm)
            {
                isUpdated = true;   
            }
            if (!isUpdated)
            {
                return BadRequest(new { message = Message.DONT_HASPERMISSION_UPDATE_PROJECT });
            }

            //thông tin cơ bản
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.ProjectId == model.Id);            
            project.ProjectTypeId = model.ProjectTypeId;
            project.Code = model.Project_Code;
            project.ProjectName = model.Project_Name;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.Description = model.Description;
            project.Revenue = model.Revenua;
            project.PM_Estimate = model.PM_Estimate ?? 0;
            project.Tester_Estimate = model.Tester_Estimate ?? 0;
            project.Brse_Estimate = model.Brse_Estimate ?? 0;
            project.Comtor_Estimate = model.Comtor_Estimate ?? 0;
            project.Developer_Estimate = model.Developer_Estimate ?? 0;
            project.PartnerId = model.PartnerId == null ? null : model.PartnerId;
            project.CustomerId = model.CustomerId == null ? null : model.CustomerId;
            project.BackLogLink = model.BacklogLink == null ? string.Empty : model.BacklogLink;

            _db.Update(project);

            //cập nhật danh sách member trong dự án
            var userprojects = await _db.UserProjects.Where(x => x.ProjectId == model.Id).ToListAsync();

            _db.UserProjects.RemoveRange(userprojects);

            if (model.UserPositions.Count() > 0)
            {
                CreateUserProjects(model.Id, model.UserPositions);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public void CreateUserProjects(int? ProjectId, List<UserPosition> UserPositions)
        {
            if (ProjectId != null && ProjectId > 0)
            {
                foreach (var position in _db.Positions.ToList())
                {
                    foreach (var userpos in UserPositions)
                    {
                        if (position.PositionId == userpos.PositionId && userpos.User_Id.Count() > 0)
                        {
                            foreach (var user_name in userpos.User_Id)
                            {
                                var userproject = new BS_UserProject()
                                {
                                    ProjectId = (int)ProjectId,
                                    UserId = user_name,
                                    PositionId = userpos.PositionId
                                };
                                _db.Add(userproject);
                            }
                        }
                    }
                }

                _db.SaveChanges();
            }
        }

        //Lấy list các dự án
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<List<ProjectViewModel>> GetAll([FromQuery] ProjectQueryParameters queryParameters)
        {
            var search = queryParameters.Search ?? "";

            var project = _db.Projects.Join(_db.ProjectTypes, project => project.ProjectTypeId,
                projectType => projectType.ProjectTypeId, (project, projectType) => new
                {
                    p = project,
                    pt = projectType
                });


            project = project.Where(x => x.p.ProjectName.Contains(search) || x.p.Code.Contains(search));

            if (queryParameters.Status != null)
            {
                project = project.Where(x => x.p.Status == queryParameters.Status);
            }

            if (queryParameters.ProjectType != null)
            {
                project = project.Where(x => x.pt.ProjectTypeId == queryParameters.ProjectType);
            }

            if (queryParameters.Project != null)
            {
                if (queryParameters.Project == 1)
                {
                    project = project.Where(x => x.p.PartnerId != null);
                }
                else
                {
                    project = project.Where(x => x.p.PartnerId == null && x.p.CustomerId == null);
                }

            }

            HttpContext.InsertParametersPaginationInHeader(project);

            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var ur = await _userManager.FindByIdAsync(userid);
            var role = await _userManager.GetRolesAsync(ur);

            var userProject = await _db.Projects.Join(_db.UserProjects,
                    project => project.ProjectId,
                    userProject => userProject.ProjectId, (project, userProject) => new
                    {
                        p = project,
                        up = userProject
                    })
                    .Where(x => x.up.UserId == userid && x.up.PositionId == Const.POSITION_PROJECT_PM).Select(i => new ProjectForReportViewModel
                    {
                        ProjectId = i.p.ProjectId,
                        Name = i.p.ProjectName
                    }).Distinct().ToListAsync();

            var data = await project.Paginage(queryParameters.PaginationVm).Select(x => new ProjectViewModel()
            {
                Project_Name = x.p.ProjectName,
                ProjectType_Name = x.pt.ProjectType_Name,
                Project_Code = x.p.Code,
                Status = x.p.Status,
                Description = x.p.Description,
                StartDate = x.p.StartDate,
                EndDate = x.p.EndDate,
                Id = x.p.ProjectId,
                IsProjectPM = userProject.Count() > 0 ? true:false
            }).OrderBy(x => x.Status).ToListAsync();            

            if (userProject.Count() > 0 && role.FirstOrDefault() != BSRole.MANAGER && role.FirstOrDefault() != BSRole.SYSADMIN)
            {
                int[] projectId = userProject.Select(x => (int)x.ProjectId).ToArray();

                data = data.Where(x => x.Id != null).Where(x => projectId.Contains(x.Id.Value)).ToList();
            }

            return data;
        }

        /// <summary>Lấy thông tin dự án có id xác định</summary>
        [HttpGet("Info/{id}")]
        [Authorize]
        public async Task<ProjectViewModel> GetById(int id)
        {
            var userproject = await _db.UserProjects.Where(x => x.ProjectId == id).Select(x => new PositionUsers()
            {
                User_Id = x.UserId,
                PositionId = x.PositionId
            }).ToListAsync();

            var results = userproject.GroupBy(x => x.PositionId).Select(up => new UserPosition()
            {
                PositionId = up.Key,
                User_Id = up.Select(u => u.User_Id).ToList()
            }).ToList();

            var project = _db.Projects.FirstOrDefault(x => x.ProjectId == id);

            var projectType = _db.ProjectTypes.FirstOrDefault(x => x.ProjectTypeId == project.ProjectTypeId);

            var projectVMs = new ProjectViewModel()
            {
                Project_Name = project.ProjectName,
                Project_Code = project.Code,
                ProjectTypeId = projectType.ProjectTypeId,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Description = project.Description,
                Id = project.ProjectId,
                PM_Estimate = project.PM_Estimate,
                Brse_Estimate = project.Brse_Estimate,
                Comtor_Estimate = project.Comtor_Estimate,
                Tester_Estimate = project.Tester_Estimate,
                Revenua = project.Revenue,
                Developer_Estimate = project.Developer_Estimate,
                UserPositions = results,
                PartnerId = project.PartnerId,
                CustomerId = project.CustomerId,
                BacklogLink = project.BackLogLink
            };

            return projectVMs;
        }

        //Khoá - mở khoá dự án
        [HttpPatch("ChangeStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] int status)
        {
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.ProjectId == id);

            if (project == null)
                return NotFound();

            project.Status = status;

            _db.Update(project);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Export")]
        public async Task<IActionResult> Export()
        {
            var userproject = await _db.UserProjects.Select(x => new PositionUsers()
            {
                User_Id = x.UserId,
                PositionId = x.PositionId
            }).ToListAsync();

            var results = userproject.GroupBy(x => x.PositionId).Select(up => new UserPosition()
            {
                PositionId = up.Key,
                User_Id = up.Select(u => u.User_Id).ToList()
            }).ToList();

            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("DanhSachProject");
                sheet.Cells[1, 1].Value = "Position Id";
                sheet.Cells[1, 2].Value = "User Id";

                int rowIndex = 2;
                foreach (var result in results)
                {
                    sheet.Cells[rowIndex, 1].Value = result.PositionId;
                    foreach (var user in result.User_Id)
                    {
                        sheet.Cells[rowIndex, 2].Value = user;
                        rowIndex++;
                    }
                }

                sheet.Cells.LoadFromCollection(results, true);

                package.Save();
            }

            stream.Position = 0;
            var fileName = Message.EXPORT_PROJECT_NAME + DateTime.Now.ToString(Message.FORMAT_DATE) + Message.FILE_TYPE_EXCEL;

            return File(stream, Message.CONTENT_TYPE_FILE_EXPORT, fileName);
        }

        [HttpGet("DashboardStat/{time}")]
        [Authorize]
        public async Task<object> GetProjectDashboardStat([FromRoute] string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));
            DateTime dateRef = new DateTime(year, month, 1);
            var day = System.DateTime.DaysInMonth(year, month);
            DateTime endDateRef = new DateTime(year, month, day);

            var lst = _db.ProjectTypes.Select(x => new
            {
                Type = x.ProjectType_Name,
                Count = x.Projects.Where(p => p.Status == Const.REPORT_STATUS_WAITING && p.StartDate <= endDateRef).Count()
            }).Distinct();

            return lst;
        }
    }
}

