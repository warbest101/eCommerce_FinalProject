using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using WebBanHang.Models;
using WebBanHang.Services;
using WebBanHang.VnPay;

namespace WebBanHang
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSession();
            services.AddMvc();
            var connection = Configuration.GetConnectionString("chuoi");
            services.AddDbContext<MyDBContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<MyDBContext>().AddDefaultTokenProviders();

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddPaging();
            #pragma warning restore CS0618 // Type or member is obsolete
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                    options => { options.LoginPath = "/dang-nhap"; options.AccessDeniedPath = "/TrangChus/Access"; }
                );
            services.ConfigureApplicationCookie(options => options.LoginPath = "/dang-nhap");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IUtils, Utils>();
            services.AddTransient<IVnPayLibrary, VnPayLibrary>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<SMSoptions>(Configuration); 

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {



                #region TrangChus

                routes.MapRoute(
                    name: "trangchu",
                    template: "",
                    defaults: new { controller = "TrangChus", action = "Index" }
                );



                routes.MapRoute(
                    name: "search",
                    template: "search",
                    defaults: new { controller = "TrangChus", action = "Search" }
                );

                routes.MapRoute(
                    name: "advancesearch",
                    template: "advance-search",
                    defaults: new { controller = "TrangChus", action = "TimKiem" }
                );


                routes.MapRoute(
                    name: "contact",
                    template: "lien-he",
                    defaults: new { controller = "TrangChus", action = "Contact" }
                );

                routes.MapRoute(
                    name: "showbaiviet",
                    template: "bai-viet",
                    defaults: new { controller = "TrangChus", action = "showbaiviet" }
                );

                routes.MapRoute(
                    name: "xembaiviet",
                    template: "bai-viet-{id}/{tieude}",
                    defaults: new { controller = "TrangChus", action = "XemBaiViet" }
                );

                routes.MapRoute(
                    name: "chinhsach",
                    template: "chinh-sach",
                    defaults: new { controller = "TrangChus", action = "ChinhSach" }
                );

                routes.MapRoute(
                    name: "dieukhoan",
                    template: "dieu-khoan",
                    defaults: new { controller = "TrangChus", action = "DieuKhoan" }
                );

                routes.MapRoute(
                    name: "dangnhap",
                    template: "dang-nhap",
                    defaults: new { controller = "TrangChus", action = "Login" }
                );

                routes.MapRoute(
                    name: "dangky",
                    template: "dang-ky",
                    defaults: new { controller = "TrangChus", action = "Create" }
                );

                routes.MapRoute(
                    name: "gioithieu",
                    template: "gioi-thieu",
                    defaults: new { controller = "TrangChus", action = "GioiThieu" }
                );

                routes.MapRoute(
                    name: "showsp",
                    template: "laptop-loai-{maloai}-{tenloai}",
                    defaults: new { controller = "TrangChus", action = "Showsp" }
                );

                routes.MapRoute(
                    name: "showspdetail",
                    template: "laptop-id-{mahh}/{tenhh}",
                    defaults: new { controller = "TrangChus", action = "Details" }
                );


                #endregion

                routes.MapRoute(
                    name: "manageaccount",
                    template: "manage-account",
                    defaults: new { controller = "ManageAccount", action = "ManageAccount" }
                );

                routes.MapRoute(
                    name: "changepassword",
                    template: "manage-account/change-password",
                    defaults: new { controller = "ManageAccount", action = "ChangePassword" }
                );

                routes.MapRoute(
                    name: "addphonenumber",
                    template: "manage-account/add-phone-number",
                    defaults: new { controller = "ManageAccount", action = "AddPhoneNumber" }
                );

                routes.MapRoute(
                    name: "verifyphonenumber",
                    template: "manage-account/verify-phone-number",
                    defaults: new { controller = "ManageAccount", action = "VerifyPhoneNumber" }
                );

                routes.MapRoute(
                    name: "admin",
                    template: "admin",
                    defaults: new { controller = "Admin", action = "Profile" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "TrangChus", action = "Index" }
                );



            });
            
        }


    }
}
