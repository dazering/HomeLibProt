using HomeLib.Infrostructure.Scanner;
using HomeLibServices;
using HomeLibServices.DataBase;
using HomeLibServices.Logger;
using HomeLibServices.Managers;
using HomeLibServices.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSignalR();
            string conString = configuration["ConnectionStrings:DefaultConnection"];
            string localRepositoryPath = configuration["LocalRepository:Path"];
            services.AddLogger<LocalLogger>();
            services.AddLibraryRepository<LibraryRepository>();
            services.AddDbContext<LibraryContext>(opt =>
            {
                opt.UseSqlServer(conString, m => m.MigrationsAssembly("HomeLib"));
            });
            services.AddBookManager(localRepositoryPath);
            services.AddSingleton<ScannerMessenger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSignalR(rout => { rout.MapHub<ScannerHub>("/scanhub"); });
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
