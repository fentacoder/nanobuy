using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nano_buy_api.Entities;
using NanoShop.PayPal;
using Stripe;

namespace nano_buy_api
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
            StripeConfiguration.ApiKey = Configuration.GetSection("AppSettings:Stripe")["SecretKey"];
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<NanoShop.Settings.AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<NanoShop.Settings.StripeSettings>(Configuration.GetSection("AppSettings:Stripe"));

            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                option.Password.RequiredLength = 6;
                option.Password.RequireUppercase = true;
                option.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddCors();
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddRazorPages();
            services.AddSession();
            services.AddSingleton<IPayPalPayment, PayPalPayment>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
