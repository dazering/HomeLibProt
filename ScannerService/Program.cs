using Microsoft.Extensions.DependencyInjection;
using ScannerService.Models;
using ScannerService.Repository;
using System;

namespace ScannerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = Configure();
            var scanner = serviceProvider.GetService<Scanner>();
            scanner.StartScanAsync();
            Console.ReadLine();
        }
        static ServiceProvider Configure()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<Scanner>();
            serviceCollection.AddDbContext<LibraryContext>();
            serviceCollection.AddTransient<ILibraryRepository, LibraryRepository>();
            return serviceCollection.BuildServiceProvider();
        }
    }
}
