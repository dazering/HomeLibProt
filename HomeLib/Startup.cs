using HomeLib.Infrostructure.Scanner;
using HomeLib.Models;
using HomeLib.Models.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeLib
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration cnf) => configuration = cnf;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<ILibraryRepository>(provider => provider.GetRequiredService<LibraryRepository>());
            services.AddTransient<LibraryRepository>();
            string conString = configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlServer(conString);
            });
            string localRepositoryPath = configuration["LocalRepository:Path"];
            services.AddScannerService(localRepositoryPath);
            services.AddDownloaderService(localRepositoryPath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
