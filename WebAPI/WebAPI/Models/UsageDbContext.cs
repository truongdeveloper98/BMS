using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UsageDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContext;

        public UsageDbContext(DbContextOptions<UsageDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContext = httpContextAccessor;
        }

        public DbSet<BS_Project> Projects { get; set; }
        public DbSet<BS_ProjectType> ProjectTypes { get; set; }
        public DbSet<BS_UserProject> UserProjects { get; set; }
        public DbSet<BS_Recruitment> Recruitments { get; set; }

        public DbSet<BS_Report> Reports { get; set; }
        public DbSet<BS_ReportOff> ReportOffs { get; set; }

        public DbSet<BS_Rate> Rates { get; set; }
        public DbSet<BS_Log> Logs { get; set; }
        public DbSet<BS_Position> Positions { get; set; }

        public DbSet<BS_Level> Levels { get; set; }
        public DbSet<BS_Language> Languages { get; set; }
        public DbSet<BS_Framework> Frameworks { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DbSet<BS_UserType> UserTypes { get; set; }
        public DbSet<BS_OffType> OffTypes { get; set; }

        public DbSet<BS_UserInfo> UserInfos { get; set; }

        public DbSet<BS_PartnerInfo> PartnerInfos { get; set; }

        public DbSet<BS_UserSalaries> UserSalaries { get; set; }

        public DbSet<BS_UserOnboard> UserOnboards { get; set; }

        public DbSet<BS_Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(builder);

            #region TableAndKeyConfiguration
            modelBuilder.Entity<IdentityRole>().ToTable("BS_Roles").Property(r => r.Name).IsRequired();
            modelBuilder.Entity<BS_UserInfo>().ToTable("BS_UserInfos").Property(r => r.UserId).IsRequired();
            modelBuilder.Entity<Document>().ToTable("Documents").Property(d => d.DocumentId).IsRequired();
            modelBuilder.Entity<BS_UserType>().ToTable("BS_UserTypes").Property(t => t.UserTypeId).IsRequired();
            modelBuilder.Entity<BS_OffType>().ToTable("BS_OffTypes").Property(o => o.Id).IsRequired();
            modelBuilder.Entity<BS_Project>().ToTable("Projects").Property(o => o.CustomerId).IsRequired(false);
            modelBuilder.Entity<BS_Project>().ToTable("Projects").Property(o => o.PartnerId).IsRequired(false);

            var tblUser = modelBuilder.Entity<ApplicationUser>().ToTable("BS_Users");
            tblUser.Property((ApplicationUser u) => u.UserName).IsRequired();

            var tblPosition = modelBuilder.Entity<BS_Position>().ToTable("BS_Positions");
            tblPosition.HasKey(x => x.PositionId);
            tblPosition.Property(x => x.PositionName).IsRequired();
            var tblUserRole = modelBuilder.Entity<IdentityUserRole<string>>().ToTable("BS_UserRoles");
            tblUserRole.HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId });

            var tblUserToken = modelBuilder.Entity<IdentityUserToken<string>>().ToTable("BS_UserTokens");
            tblUserToken.HasKey(r => new { LoginProvider = r.LoginProvider, Name = r.Name, UserId = r.UserId });

            var tblUserLogin = modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("BS_UserLogins");
            tblUserLogin.HasKey((IdentityUserLogin<string> l) =>
                    new
                    {
                        UserId = l.UserId,
                        LoginProvider = l.LoginProvider,
                        ProviderKey = l.ProviderKey
                    });

            #endregion

            #region SetRelation
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.Reports)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .HasPrincipalKey(u => u.Id);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.ReportOffs)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .HasPrincipalKey(u => u.Id);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.UserProjects)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .HasPrincipalKey(u => u.Id);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(v => v.UserInfo)
                .WithOne(u => u.User)
                .HasForeignKey<BS_UserInfo>(u => u.UserId)
                .HasPrincipalKey<ApplicationUser>(v => v.Id);

            modelBuilder.Entity<BS_Project>()
                .HasMany(p => p.UserProjects)
                .WithOne(up => up.Project).IsRequired()
                .HasForeignKey(up => up.ProjectId)
                .HasPrincipalKey(p => p.ProjectId);

            modelBuilder.Entity<BS_Position>()
              .HasMany(p => p.UserProjects)
              .WithOne(up => up.Position).IsRequired()
              .HasForeignKey(up => up.PositionId)
              .HasPrincipalKey(p => p.PositionId);

            modelBuilder.Entity<BS_Report>()
                .HasOne(rp => rp.Project)
                .WithMany(up => up.Reports)
                .HasForeignKey(rp => rp.ProjectId)
                .HasPrincipalKey(up => up.ProjectId);

            modelBuilder.Entity<BS_UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId, up.PositionId });

            modelBuilder.Entity<BS_ProjectType>()
                .HasMany(p => p.Projects)
                .WithOne(up => up.ProjectType).IsRequired()
                .HasForeignKey(up => up.ProjectTypeId)
                .HasPrincipalKey(p => p.ProjectTypeId);

            modelBuilder.Entity<BS_UserType>()
                .HasMany(p => p.UserInfos)
                .WithOne(up => up.UserType).IsRequired()
                .HasForeignKey(up => up.TypeId)
                .HasPrincipalKey(p => p.UserTypeId);

            modelBuilder.Entity<BS_OffType>()
                .HasMany(p => p.ReportOffs)
                .WithOne(up => up.OffType).IsRequired()
                .HasForeignKey(up => up.OffTypeId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.UserSalaries)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .HasPrincipalKey(u => u.Id);

            modelBuilder.Entity<BS_PartnerInfo>()
                .HasMany(p => p.CustomerProjects)
                .WithOne(up => up.Customer)
                .HasForeignKey(up => up.CustomerId)
                .HasPrincipalKey(p => p.PartnerId);

            modelBuilder.Entity<BS_PartnerInfo>()
                .HasMany(p => p.PartnerProjects)
                .WithOne(up => up.Partner)
                .HasForeignKey(up => up.PartnerId)
                .HasPrincipalKey(p => p.PartnerId);

            #endregion

            #region SeedData
            modelBuilder.Entity<BS_UserType>().HasData(
                new BS_UserType { UserTypeId = 1, Name = "Intern" },
                new BS_UserType { UserTypeId = 2, Name = "Probation" },
                new BS_UserType { UserTypeId = 3, Name = "Official" },
                new BS_UserType { UserTypeId = 4, Name = "Fresher" }
            );
            modelBuilder.Entity<BS_OffType>().HasData(
                new BS_OffType { Id = 0, Name = "Nghỉ phép" },
                new BS_OffType { Id = 1, Name = "Nghỉ không lương" },
                new BS_OffType { Id = 2, Name = "Nghỉ đặc biệt" }
            );
            #endregion

        }



        #region Override database mothod

        public override EntityEntry Add(object entity)
        {
            return base.Add(entity);
        }

        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.Add(entity);

            var user = _httpContext.HttpContext != null ? _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) : null;
            item.Date_Created = DateTime.Now;
            item.Created_By = user?.Value;
            item.Last_Updated = DateTime.Now;
            item.Updated_By = user?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChangesTEntity

            return base.Add(entity);
        }

        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.AddAsync(entity, cancellationToken);

            var user = _httpContext.HttpContext != null ? _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) : null;
            item.Date_Created = DateTime.Now;
            item.Created_By = user?.Value;
            item.Last_Updated = DateTime.Now;
            item.Updated_By = user?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChangesTEntity

            return base.AddAsync(entity, cancellationToken);
        }

        public override void AddRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                var item = entity as BaseModels;
                if (item == null)
                {
                    base.Add(entity);
                }
                else
                {
                    var user = _httpContext.HttpContext != null ? _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) : null;
                    item.Date_Created = DateTime.Now;
                    item.Created_By = user?.Value;
                    item.Last_Updated = DateTime.Now;
                    item.Updated_By = user?.Value;
                }
            }
            base.AddRange(entities);
        }

        public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                var item = entity as BaseModels;
                if (item == null)
                {
                    base.AddAsync(entity);
                }
                else
                {
                    var user = _httpContext.HttpContext != null ? _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) : null;
                    item.Date_Created = DateTime.Now;
                    item.Created_By = user?.Value;
                    item.Last_Updated = DateTime.Now;
                    item.Updated_By = user?.Value;
                }
            }
            return base.AddRangeAsync(entities, cancellationToken);
        }

        public override EntityEntry Update(object entity)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.Update(entity);

            item.Last_Updated = DateTime.Now;
            item.Updated_By = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChanges();

            return base.Update(item);
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.Update(entity);

            item.Last_Updated = DateTime.Now;
            if (_httpContext.HttpContext != null)
                item.Updated_By = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChangesTEntity

            return base.Update<TEntity>(entity);
        }

        public override void UpdateRange(IEnumerable<object> entities)
        {
            base.UpdateRange(entities);
        }

        public override EntityEntry Remove(object entity)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.Remove(entity);

            item.IsDeleted = true;
            item.Last_Updated = DateTime.Now;
            if (_httpContext.HttpContext != null)
                item.Updated_By = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChanges();

            return Update(item);
        }

        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            var item = entity as BaseModels;
            if (item == null)
                return base.Remove(entity);

            item.IsDeleted = true;
            item.Last_Updated = DateTime.Now;
            if (_httpContext.HttpContext != null)
                item.Updated_By = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Entry(item).State = EntityState.Modified;
            //SaveChanges();

            return Update<TEntity>(entity);
        }

        public override void RemoveRange(IEnumerable<object> entities)
        {
            base.RemoveRange(entities);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion Override database mothod

    }
}
