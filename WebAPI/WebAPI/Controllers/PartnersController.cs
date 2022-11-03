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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBaseBS
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PartnersController(UsageDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        //Lấy list các partner/customer
        [HttpGet("GetAll")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<List<PartnerViewModels>> GetAll([FromQuery] PartnerQueryParameters queryParameters)
        {
            var search = queryParameters.Search ?? "";

            // get all data
            var partner = queryParameters.IsPartner == true ? _db.PartnerInfos.Where(x => x.IsPartner == true).Select(x => x)
                                                              : _db.PartnerInfos.Where(x => x.IsPartner == false).Select(x => x);

            partner = partner.Where(x => x.PartnerName.Contains(search) || x.Address.Contains(search));

            HttpContext.InsertParametersPaginationInHeader(partner);

            var data = await partner.Paginage(queryParameters.PaginationVm).Select(x => new PartnerViewModels()
            {
                PartnerName = x.PartnerName,
                Address = x.Address,
                Website = x.Website,
                IsDelete = x.IsDeleted,
                Note = x.Note,
                Vote = x.Vote,
                PartnerId = x.PartnerId,
            }).OrderBy(x => x.IsDelete).ToListAsync();

            return data;
        }

        //Lấy danh sách partner
        [HttpGet]
        [Route("GetPartner/{type}")]        
        [Authorize]
        public async Task<ActionResult<List<PartnerViewModels>>> GetAllPartner(int type)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            List<PartnerViewModels> models;
            if (type == 0)
            {
                models = await _db.PartnerInfos.Where(u => u.IsDeleted == false && u.IsPartner == true)
                    .Select(x => new PartnerViewModels()
                    {
                        PartnerId = x.PartnerId,
                        PartnerName = x.PartnerName,
                        Website = x.Website
                    }).ToListAsync();
            }
            else
            {
                models = await _db.PartnerInfos.Where(u => u.IsDeleted == false && u.IsPartner == false)
                    .Select(x => new PartnerViewModels()
                    {
                        PartnerId = x.PartnerId,
                        PartnerName = x.PartnerName,
                        Website = x.Website
                    }).ToListAsync();
            }

            return models;
        }

        //Tạo Customer mới
        [HttpPost]
        [Route("CreatePartner")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<ActionResult> CreateCustomer([FromBody] CreatePartnerViewModels request)
        {
            var partnerInfo = new BS_PartnerInfo();
            partnerInfo.PartnerName = request.PartnerName;
            partnerInfo.Address = request.Address;
            partnerInfo.Note = request.Note;
            partnerInfo.Vote = request.Vote;
            partnerInfo.Website = request.Website;
            partnerInfo.IsPartner = request.IsPartner;
            partnerInfo.IsDeleted = false;

            await _db.PartnerInfos.AddAsync(partnerInfo);

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Cập nhật thông tin partner
        [HttpPut("UpdatePartner")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> UpdatePartner([FromBody] PartnerViewModels request)
        {
            var partnerInfo = await _db.PartnerInfos.FirstOrDefaultAsync(x => x.PartnerId == request.PartnerId);

            if (partnerInfo == null)
            {
                return NotFound();
            }

            //thông tin cơ bản
            partnerInfo.PartnerName = request.PartnerName;
            partnerInfo.Address = request.Address;
            partnerInfo.Note = request.Note;
            partnerInfo.Vote = request.Vote;
            partnerInfo.Website = request.Website;

            _db.Update(partnerInfo);

            await _db.SaveChangesAsync();

            return Ok();
        }

        //Lấy thông tin partner có id xác định
        [HttpGet("Info/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public PartnerViewModels GetById(int id)
        {
            var partnerInfo = _db.PartnerInfos.FirstOrDefault(x => x.PartnerId == id);

            var partnerViewModels = new PartnerViewModels()
            {
                PartnerName = partnerInfo.PartnerName,
                Address = partnerInfo.Address,
                Website = partnerInfo.Website,
                Note = partnerInfo.Note,
                Vote = partnerInfo.Vote,

            };

            return partnerViewModels;
        }

        //Khoá - mở khoá partner
        [HttpPatch("ChangeStatus/{id}")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] bool status)
        {
            var partnerinfo = await _db.PartnerInfos.FirstOrDefaultAsync(x => x.PartnerId == id);

            if (partnerinfo == null)
                return NotFound();

            partnerinfo.IsDeleted = !status;

            _db.Update(partnerinfo);

            await _db.SaveChangesAsync();

            return Ok();
        }

    }
}
