using HomeLib.Infrostructure.Scanner;
using HomeLibServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddDefaultLibraryServices(conString, localRepositoryPath, (provider, book) =>
            {
                book.ScannerMessage += provider.GetService<ScannerHub>().MessageRecived;
            });

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
