using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Vxp.Common;
using Vxp.Data;
using Vxp.Data.Common;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Data.Seeding;
using Vxp.Services.Data.BankAccounts;
using Vxp.Services.Data.Settings;
using Vxp.Services.Data.Users;
using Vxp.Services.Mapping;
using Vxp.Services.Messaging;
using Vxp.Services.Models.Administration.Users;
using Vxp.Web.ViewModels;
using Vxp.Web.ViewModels.Administration.Dashboard;

namespace Vxp.Web
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Framework services
            // TODO: Add pooling when this bug is fixed: https://github.com/aspnet/EntityFrameworkCore/issues/9741
            services.AddDbContext<ApplicationDbContext>(
                options => options.
                    // UseLazyLoadingProxies().
                    UseSqlServer(this._configuration.GetConnectionString("DefaultConnection")));

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4);

            services
                .AddMvc(options =>
                {
                    // options.ModelBinderProviders.Insert(0, new BankAccountToSelectListItemModelBinderProvider());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                    options.ConsentCookie.Name = ".AspNetCore.ConsentCookie";
                });

            services.AddResponseCompression(opt => opt.EnableForHttps = true);

            services.AddSingleton(this._configuration);

            // Identity stores
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            // services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISmsSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IDistributorsService, DistributorsService>();
            services.AddTransient<IEmailSender, SendGridEmailSender>(ctx =>
            {
                var logger = ctx.GetService<ILoggerFactory>();

                return new SendGridEmailSender(
                    logger,
                    this._configuration["Authentication:SendGridApiKey"],
                    GlobalConstants.Email.SystemEmailSendFromEmail,
                    GlobalConstants.Email.SystemEmailSendFromName);
            });

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IBankAccountsService, BankAccountsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(IndexViewModel).GetTypeInfo().Assembly,
                typeof(CreateUserServiceModel).GetTypeInfo().Assembly);


            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {


                if (env.IsDevelopment())
                {
                    //var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //dbContext.Database.EnsureDeleted();
                    //dbContext.Database.Migrate();
                    //new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
                }


            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseResponseCompression();

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
