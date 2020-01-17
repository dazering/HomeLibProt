using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HomeLib.Models.Repository;

namespace HomeLib.Infrostructure.Scanner
{
    public static class ServiceProviderExtensions
    {
        public static void AddScannerService(this IServiceCollection services)
        {
            services.AddSingleton<Scanner>(provider =>new Scanner(provider));
        }
        public static void AddDownloaderService(this IServiceCollection services, string path)
        {
            services.AddTransient<Downloader>(provider => new Downloader(path));
        }
    }
}
