//Controller chứa các function liên quan tới quản lý member

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.HubConfig;
using WebAPI.Models;
using WebAPI.Models.ViewModels;
using WebAPI.Utils;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBaseBS
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<BMSHub, IHubClient> _hubContext;

        public UsersController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, UsageDbContext context,
            IHubContext<BMSHub, IHubClient> hubContext,
            RoleManager<IdentityRole> roleManager) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        //Lấy thông tin của member có id xác định
        [HttpGet]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        [Route("GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var ur = await _userManager.FindByIdAsync(id);
            BS_UserInfo infos = ur.UserInfo;
            var roles = await _userManager.GetRolesAsync(ur);
            var role = roles.FirstOrDefault();

            role = role == Const.ROLE_DEV ? Const.ROLE_MEMBER : role == Const.ROLE_HR ? Const.ROLE_OTHER : role;

            var thisMonth = DateTime.Now.Month;
            var result = await (from u in _db.Users
                                where u.Id == id
                                select new UserViewModel
                                {
                                    Id = u.Id,
                                    First_Name = u.First_Name,
                                    Last_Name = u.Last_Name,
                                    Birth_Date = Usage.Date2Utc(u.Birth_Date),
                                    Address = u.Address,
                                    Avatar = u.Avatar,
                                    Email = u.Email,
                                    Role = role,
                                    Start_Date = Usage.Date2Utc(u.Start_Date),
                                    End_Date = Usage.Date2Utc(u.End_Date),
                                    PhoneNumber = u.PhoneNumber,
                                    Info = new UserInfoVM
                                    {
                                        UserId = u.UserInfo.UserId,
                                        Level = u.UserInfo.Level,
                                        Team = u.UserInfo.Team,
                                        Position = u.UserInfo.Position,
                                        Department = u.UserInfo.Department,
                                        TypeId = u.UserInfo.TypeId,
                                        IsPending = u.UserInfo.IsPending,
                                        EffortFree = u.UserInfo.EffortFree,
                                        Company = u.UserInfo.CompanyId,
                                        CVLink = u.UserInfo.CVLink,
                                        PendingStart = Usage.Date2Utc(u.UserInfo.PendingStart) ,
                                        PendingEnd = Usage.Date2Utc(u.UserInfo.PendingEnd),
                                        TotalLeaveDay = u.UserInfo.ThisYearLeaveDay + u.UserInfo.LastYearLeaveDay,
                                        OccupiedLeaveDay = u.UserInfo.OccupiedLeaveDay,
                                        AvaiableLeaveDay = (u.UserInfo.ThisYearLeaveDay + u.UserInfo.LastYearLeaveDay) - (12 - thisMonth) > 0 ?
                                                  (u.UserInfo.ThisYearLeaveDay + u.UserInfo.LastYearLeaveDay) - (12 - thisMonth) :
                                                   0//phép còn lại trong tháng
                                    }
                                }).FirstOrDefaultAsync();


            if (result == null) 
                return NotFound();
            //result.MonthDayOff = Math.Round(result.MonthDayOff, 2);
            result.Info.AvaiableLeaveDay = Math.Round(result.Info.AvaiableLeaveDay, 2);

            return Ok(result);
        }

        //Tìm kiếm danh sách member theo diều kiện
        [HttpGet]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        [Route("FilterUsers")]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers([FromQuery] UserQueryParameters queryParameters)
       {
            var queryable = _db.Users.Join(_db.UserInfos, u => u.Id, ui => ui.UserId, (u, ui) => new { u, ui });

            if (!string.IsNullOrEmpty(queryParameters.Search))
            {
                queryable = queryable.Where(x => x.u.Email.Contains(queryParameters.Search)
                || x.u.First_Name.Contains(queryParameters.Search)
                || x.u.Last_Name.Contains(queryParameters.Search));
            }

            if (int.Parse(queryParameters.Department)>-1)
            {
                queryable = queryable.Where(x => x.ui.Department == int.Parse(queryParameters.Department));
            }

            HttpContext.InsertParametersPaginationInHeader(queryable);

            var userPage = await queryable.OrderBy(x => x.u.UserName).Paginage(queryParameters.PaginationVm)
                 .Select(x => new UserViewModel()
                 {
                     Id = x.u.Id,
                     Avatar = x.u.Avatar,
                     DisplayName = x.u.DisplayName,
                     Email = x.u.Email,
                     Start_Date = Usage.Date2Utc(x.u.Start_Date),
                     End_Date = Usage.Date2Utc(x.u.End_Date),
                     IsDeleted = x.u.IsDeleted,
                     Info = new UserInfoVM()
                     {
                         Position = x.ui.Position,
                         TypeId = x.ui.TypeId,
                         Department = x.ui.Department
                     }
                 }).OrderBy(x => x.IsDeleted).ToListAsync();

            foreach (var user in userPage)
            {
                var users = await _userManager.FindByIdAsync(user.Id);
                var role = (await _userManager.GetRolesAsync(users)).FirstOrDefault();
                user.Role = role == Const.ROLE_DEV ? Const.ROLE_MEMBER : role == Const.ROLE_HR ?
                                    Const.ROLE_OTHER : role == Const.ROLE_LIST[0]? Const.ROLE_LIST[0]:
                                    role == Const.ROLE_LIST[1] ? Const.ROLE_LIST[1]: role == Const.ROLE_PARTNER 
                                    ? Const.ROLE_PARTNER : Const.ROLE_MEMBER;
            }

            return userPage;
        }

        //Lấy danh sách toàn bộ member
        [HttpGet]
        [Route("GetAlls")]
        [Authorize]
        public async Task<ActionResult<List<UserViewModel>>> GetAlls()
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            return await _userManager.Users.Join(_db.UserInfos, u => u.Id, ui => ui.UserId, (u, ui) => new { u, ui })
                .Where(x => x.u.IsDeleted == false).Select(x => new UserViewModel()
            {
                Id = x.u.Id,
                Avatar = x.u.Avatar != null ? url + '/' + x.u.Avatar.Replace("\\", "/") : null,
                DisplayName = x.u.DisplayName,
                Email = x.u.Email,
                Start_Date = Usage.Date2Utc(x.u.Start_Date),
                End_Date = Usage.Date2Utc(x.u.End_Date),
                IsDeleted = x.u.IsDeleted,
                Info = new UserInfoVM
                {
                    Department = x.ui.Department,
                    TypeId = x.ui.TypeId
                }
            }).OrderBy(x => x.IsDeleted).ToListAsync();
        }

        //Tìm kiếm theo tên
        [HttpPost("SearchByName")]
        [Authorize]
        public async Task<ActionResult<List<GetUserForReport>>> SearchByName([FromBody] string displaynname)
        {
            if (string.IsNullOrWhiteSpace(displaynname))
            {
                return NoContent();
            }

            return await _userManager.Users
                .Where(x => x.DisplayName.Contains(displaynname))
                .OrderBy(x => x.DisplayName)
                .Select(x => new GetUserForReport { UserId = x.Id, DisplayName = x.DisplayName })
                .Take(5)
                .ToListAsync();
        }

        //Lấy danh sách các role
        [HttpGet]
        [Route("GetRoles")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.Where(role => Const.ROLE_LIST.Any(r => r == role.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name == Const.ROLE_LIST[2] ?
                                        Const.ROLE_MEMBER : x.Name == Const.ROLE_LIST[3] ?
                                        Const.ROLE_OTHER : x.Name
                }).ToListAsync();

            return Ok(roles);
        }

        [HttpGet]
        [Route("GetUserForReport")]
        [Authorize]
        public async Task<List<GetUserForReport>> GetUserForReport()
        {
            return await _userManager.Users.Select(x => new GetUserForReport()
            {
                UserId = x.Id,
                DisplayName = x.DisplayName
            }).ToListAsync();
        }

        //Cập nhật thông tin member
        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(model.Id);
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null)
            {
                return NotFound();
            }

            //cập nhật thông tin cơ bản
            user.DisplayName = model.First_Name + " " + model.Last_Name;
            user.First_Name = model.First_Name;
            user.Last_Name = model.Last_Name;
            user.Birth_Date = Usage.Date2Utc(model.Birth_Date);
            user.Start_Date = Usage.Date2Utc(model.Start_Date);
            user.End_Date = Usage.Date2Utc(model.End_Date); 
            user.Address = model.Address;
            user.Avatar = model.Avatar;
            //user.Department = model.Department;
            user.PhoneNumber = model.PhoneNumber;
            //user.UserTypeId = model.TypeId;
            user.Last_Updated = DateTime.Now;
            user.Updated_By = userid;
            if (roles.FirstOrDefault() != model.Role)
            {
                user.LockoutEnabled = true;
            }

            var result = await _userManager.UpdateAsync(user);

            //cập nhật role
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            model.Role = model.Role == Const.ROLE_MEMBER ? Const.ROLE_DEV : model.Role == Const.ROLE_OTHER ? Const.ROLE_HR : model.Role;
            await _userManager.AddToRoleAsync(user, model.Role);

            var info = _db.UserInfos.Where(x => x.UserId == model.Info.UserId).FirstOrDefault();

            info.Level = model.Info.Level;
            info.Team = model.Info.Team;
            info.Department = model.Info.Department;
            info.TypeId = model.Info.TypeId;
            info.IsPending = model.Info.IsPending;
            info.Company = model.Info.Company.ToString();
            info.CompanyId = model.Info.Company;
            info.Position = model.Info.Position;
            info.CVLink = model.Info.CVLink;

            if (model.Info.IsPending)
            {
                info.PendingStart = model.Info.PendingStart;
                info.PendingEnd = model.Info.PendingEnd;
                info.EffortFree = model.Info.EffortFree;
            }
            else
            {
                info.PendingStart = null;
                info.PendingEnd = null;
                info.EffortFree = 0;
            }
            info.OccupiedLeaveDay = model.Info.OccupiedLeaveDay;
            //check xem member đủ 5 năm hay chưa
            bool over5Year = (DateTime.Now.Year - user.Start_Date.Value.Year > 5 ||
                             (DateTime.Now.Year - user.Start_Date.Value.Year == 5 &&
                              DateTime.Now.Month >= user.Start_Date.Value.Month)
                              );
            double thisYearOff = over5Year ? 13 : 12;

            //Cập nhật ngày nghỉ phép
            if (model.Info.TotalLeaveDay >= thisYearOff)
            {
                info.ThisYearLeaveDay = thisYearOff;
                info.LastYearLeaveDay = Math.Round(model.Info.TotalLeaveDay - thisYearOff, 2);
            }
            else
            {
                info.ThisYearLeaveDay = model.Info.TotalLeaveDay;
                info.LastYearLeaveDay = 0;
            }

            if (model.Info.TypeId != Const.USER_TYPE_OFFICIAL)
            {
                info.ThisYearLeaveDay = 0;
                info.LastYearLeaveDay = 0;
                info.OccupiedLeaveDay = 0;
            }

            _db.Update(info);
            //Remove from Project for non-manufacture team
            if (model.Info.Department >= Const.MAX_DEP_MANUFACTORY)
            {
                var userPrj = from up in _db.UserProjects where up.UserId == model.Id select up;
                _db.RemoveRange(userPrj);
            }
            await _db.SaveChangesAsync();

            if (roles.FirstOrDefault() != model.Role)
            {
                await _hubContext.Clients.All.BroadcastMessage(model.Id);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetProfile")]
        [Authorize]
        public async Task<UserViewModel> GetProfile()
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            var u = await _userManager.Users.Where(x => x.Id == userid).Select(x => new UserViewModel
            {
                Id = x.Id,
                First_Name = x.First_Name,
                Last_Name = x.Last_Name,
                Email = x.Email,
                Birth_Date = Usage.Date2Utc(x.Birth_Date),
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                Avatar = x.Avatar != null ? url + '/' + x.Avatar.Replace("\\", "/") : null,
            }).FirstOrDefaultAsync();

            return u;
        }


        //Cập nhật avatar
        [HttpPost]
        [Route("UpdateAvatar/{id}")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile profileImage)
        {
            var validExtensions = new List<string>
            {
                ".jpeg",
                ".png",
                ".gif",
                ".jpg"
            };

            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName);

                if (validExtensions.Contains(extension))
                {
                    double fileSizeKB = profileImage.Length / 1024;
                    if (fileSizeKB > 800)
                    {
                        return BadRequest(Message.PASS_SIZE_IMAGE);
                    }

                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        var avatarOld = user.Avatar;
                        var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                        var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                        LocalStorageImage local = new LocalStorageImage(url);

                        var fileImagePath = await local.Upload(profileImage, fileName);

                        user.Avatar = fileImagePath;
                        var result = await _userManager.UpdateAsync(user);

                        if (avatarOld != null && result.Succeeded)
                        {
                            var imageOld = avatarOld.Split('\\');

                            var rootPath = _webHostEnvironment.WebRootPath;
                            FileStorage file = new FileStorage(url, rootPath);
                            await file.DeleteFile(imageOld[2], @"Resources\Images");
                        }

                        return Ok(url + '/' + fileImagePath);
                    }
                }
                return BadRequest(Message.INVALID_FORMAT_IMAGE);
            }
            return NotFound(Message.NOT_FOUND);
        }

        [HttpPut]
        [Route("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> AccountSetting([FromBody] ApplicationUser model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return NotFound();
            }

            user.DisplayName = model.First_Name + " " + model.Last_Name;
            user.First_Name = model.First_Name;
            user.Last_Name = model.Last_Name;
            user.Birth_Date = Usage.Date2Utc(model.Birth_Date);
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.Last_Updated = DateTime.Now;
            user.Updated_By = userid;

            var result = await _userManager.UpdateAsync(user);

            return Ok(result);
        }

        //Đổi mật khẩu
        [HttpPut]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePwViewModel request)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                return NotFound();
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            if (checkPassword)
            {
                user.Last_Updated = DateTime.Now;
                user.Updated_By = userid;
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                return Ok(result);
            }
            else
            {
                //Mật khẩu cũ không chính xác
                return BadRequest(new { result = 0 });
            }
        }

        //Khoá - mở khoá tài khoản member
        [HttpPatch]
        [Route("UpdateStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] DateTime endDate)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //IsDeleted = 1 : acc bị khoá
            user.IsDeleted = !user.IsDeleted;

            if (user.IsDeleted) user.End_Date = endDate;
            else user.End_Date = null;

            user.Last_Updated = DateTime.Now;
            user.Updated_By = userid;

            var result = await _userManager.UpdateAsync(user);

            return Ok(result);
        }

        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(claims);
        }

        //Lấy danh sách các type (thực tập - thử việc - chính thức -vv
        [HttpGet("GetTypeList")]
        [Authorize]
        public async Task<List<UserTypeViewModel>> GetTypeList()
        {
            var result = await _db.UserTypes.Select(x => new UserTypeViewModel
            {
                Id = x.UserTypeId,
                Name = x.Name
            }).OrderBy(x => x.Id).ToListAsync();

            return result;
        }

        //Member lấy data nghỉ phép của mình
        [HttpGet]
        [Route("MyLeaveDayData")]
        [Authorize]
        public async Task<LeaveDayVM> GetMyDayOffData()
        {
            var id = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            var offday = _db.ReportOffs.Where(x => x.UserId == id &&
                                                x.IsDeleted == false && //chưa xoá
                                                x.Status != Const.REPORT_STATUS_APPROVED && //chưa duyệt
                                                x.OffTypeId == 0).Sum(x => x.OffDay);

            var result = _db.UserInfos.Where(v => v.UserId == id).Select(v => new LeaveDayVM
            {
                UserId = v.UserId,
                ThisYearTotal = v.ThisYearLeaveDay + v.LastYearLeaveDay - offday,
                Occupied = v.OccupiedLeaveDay + offday,
                Avaiable = v.ThisYearLeaveDay + v.LastYearLeaveDay - (12 - DateTime.Now.Month) - offday > 0 ?
                           v.ThisYearLeaveDay + v.LastYearLeaveDay - (12 - DateTime.Now.Month) - offday :
                           0
            }).FirstOrDefault();

            if (result != null)
            {
                result.Avaiable = Math.Round(result.Avaiable, 2);
            }

            return result;
        }

        [HttpGet("MemberStat/{time}")]
        [Authorize]
        public async Task<object> GetMemberStat([FromRoute] string time)
        {
            int month = int.Parse(time.Substring(0, time.Length - 4));
            int year = int.Parse(time.Substring(time.Length - 4));
            DateTime startDateRef = new DateTime(year, month, 1);
            var day = System.DateTime.DaysInMonth(year, month);
            DateTime endDateRef = new DateTime(year, month, day).ToUniversalTime();
            
            var lst = await _db.Roles.Select(x => new { Role = x.Name, Count = 0 }).ToListAsync();

            var userList = await _db.Users.Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur }).
                Join(_db.Roles, x => x.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Where(x => x.r.Name != "SysAdmin" //loại trừ Admin
                                       //&& (x.ur.u.End_Date == null | x.ur.u.End_Date >= endDateRef) //nghỉ sau ngày cuối tháng
                                       //&& x.ur.u.Start_Date <= endDateRef //join trước cuối tháng
                                       && x.ur.u.UserInfo.Department < Const.MAX_DEP_MANUFACTORY
                                       && !x.ur.u.IsDeleted) //chỉ lấy account team sản xuất
                                  .Select(i => new
                                  {
                                      Role = i.r.Name,
                                      UserId = i.ur.u.Id,
                                      Department = i.ur.u.UserInfo.Department,
                                      Contract = i.ur.u.UserInfo.TypeId
                                  }).Distinct().ToListAsync();

            var res = new
            {
                Total = userList.Count,
                BeetSoft = userList.Where(x => x.Department != 2).Count(),
                Partner = userList.Where(x => x.Department == 2).Count(),
                Chartdata = userList.Where(x => x.Department != 2).GroupBy(x => x.Role).
                             Select(y => new
                             {
                                 Role = y.Key,
                                 Count = userList.Where(a => a.Role == y.Key && a.Department != 2).Select(a => a).Count()
                             }),
                ChartDepartData = userList.GroupBy(x => x.Department).Select(y => new
                {
                    Department = y.Key,
                    Count = userList.Where(a => a.Department == y.Key).Select(a => a).Count()
                }),
                ChartContractData = userList.Where(x => x.Department != 2).GroupBy(x => x.Contract).Select(y => new
                {
                    Contract = y.Key,
                    Count = userList.Where(a => a.Contract == y.Key && a.Department != 2).Select(a => a).Count()
                })
            };

            return res;
        }

        // Tạo thành viên onboard mới
        [HttpPost]
        [Route("CreateUserOnboard")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<ActionResult> CreateUserOnboard([FromBody] UserOnboardViewModel request)
        {
            var userOnboard = new BS_UserOnboard();
            userOnboard.FullName = request.FullName;
            userOnboard.Position = request.Position;
            userOnboard.Language = request.Language;
            userOnboard.Level = request.Level;
            userOnboard.OnboardDate = request.OnboardDate;
            userOnboard.Note = request.Note;

            _db.Add(userOnboard);
            await _db.SaveChangesAsync();

            return Ok();
        }

        // Cập nhật thông tin thành viên onboard
        [HttpPut("UpdateUserOnboard")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN + "," + BSRole.HR)]
        public async Task<IActionResult> UpdateUserOnboard([FromBody] UserOnboardViewModel request)
        {
            var userOnboard = await _db.UserOnboards.FirstOrDefaultAsync(x => x.Id == request.Id);

            //thông tin cơ bản
            userOnboard.FullName = request.FullName;
            userOnboard.Position = request.Position;
            userOnboard.Language = request.Language;
            userOnboard.Level = request.Level;
            userOnboard.OnboardDate = request.OnboardDate;
            userOnboard.Note = request.Note;

            _db.Update(userOnboard);

            await _db.SaveChangesAsync();

            return Ok();
        }

        // Lấy list các thành viên onboard
        [HttpGet("GetUsersOnboard")]
        [Authorize]
        public async Task<List<UserOnboardViewModel>> GetUsersOnboard([FromQuery] UserOnboardQueryParameters queryParameters)
        {
            var search = queryParameters.Search ?? "";

            var queryable = _db.UserOnboards.Where(x => x.FullName.Contains(search) || x.Language.Contains(search));

            HttpContext.InsertParametersPaginationInHeader(queryable);

            var data = await queryable.Paginage(queryParameters.PaginationVm).Select(x => new UserOnboardViewModel()
            {
                FullName = x.FullName,
                Position = x.Position,
                Language = x.Language,
                Level = x.Level,
                OnboardDate = x.OnboardDate,
                Note = x.Note,
                IsDeleted = x.IsDeleted,
                Id = x.Id,
            }).ToListAsync();

            return data;
        }

        //Lấy thông tin thành viên onboard có id xác định
        [HttpGet("GetUserOnboard/{id}")]
        [Authorize]
        public async Task<UserOnboardViewModel> GetUserOnboard(int id)
        {
            var userOnboard = await _db.UserOnboards.FirstOrDefaultAsync(x => x.Id == id);

            var userOnboardViewModel = new UserOnboardViewModel()
            {
                FullName = userOnboard.FullName,
                Position = userOnboard.Position,
                Language = userOnboard.Language,
                Level = userOnboard.Level,
                OnboardDate = userOnboard.OnboardDate,
                Note = userOnboard.Note,
                Id = userOnboard.Id,
            };

            return userOnboardViewModel;
        }

        //Khoá - mở khoá thành viên onboard
        [HttpPatch("ChangeUserOnboardStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> ChangeUserOnboardStatus([FromRoute] int id, [FromBody] bool isDeleted)
        {
            var userOnboard = await _db.UserOnboards.FirstOrDefaultAsync(x => x.Id == id);

            if (userOnboard == null)
                return NotFound();

            userOnboard.IsDeleted = isDeleted;

            _db.Update(userOnboard);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy danh sách partner
        [HttpGet("Companies")]        
        [Authorize]
        public async Task<ActionResult<List<CompanyViewModel>>> Companies()
        {
            List<CompanyViewModel> lstCompanies;
            lstCompanies = await _db.PartnerInfos.Where(u => u.IsDeleted == false && u.IsPartner == true)
                    .Select(x => new CompanyViewModel()
                    {
                        CompanyId = x.PartnerId,
                        CompanyName = x.PartnerName
                    }).ToListAsync();

            lstCompanies.Insert(0, new CompanyViewModel()
            {
                CompanyId = Const.DEFAULT_COMPANY_ID,
                CompanyName = Const.DEFAULT_COMPANY_NAME
            });

            return lstCompanies;
        }
    }
}
