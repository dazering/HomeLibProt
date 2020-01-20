using HomeLibServices.DataBase;
using HomeLibServices.Logger;
using HomeLibServices.Managers;
using HomeLibServices.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HomeLibServices
{
    public static class Extensions
    {
        /// <summary>
        /// Add to DI container defaults implementations: LibraryContext, LibraryRepository, LocalLogger and BookManager
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="dbConnection"></param>
        /// <param name="pathToLocalRepository"></param>
        public static void AddDefaultLibraryServices(this IServiceCollection collection, string dbConnection, string pathToLocalRepository)
        {
            collection.AddDbContext<LibraryContext>(opt => opt.UseSqlServer(dbConnection));
            collection.AddScoped<ILibraryRepository, LibraryRepository>();
            collection.AddTransient<ILogger, LocalLogger>();
            collection.AddBookManager(pathToLocalRepository);
        }

        /// <summary>
        /// Add to DI container BookManager
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="pathToLocalRepository"></param>
        public static void AddBookManager(this IServiceCollection collection, string pathToLocalRepository)
        {
            collection.AddSingleton<BookManager>(provider => new BookManager(pathToLocalRepository, provider));
        }

        /// <summary>
        /// Add to DI container user implementation of DbContext
        /// </summary>
        /// <typeparam name="T">Must inherit from DbContext</typeparam>
        /// <param name="collection"></param>
        /// <param name="dbConnection">connection string</param>
        public static void AddDbContext<T>(this IServiceCollection collection, string dbConnection) where T : DbContext
        {
            collection.AddDbContext<T>(opt => opt.UseSqlServer(dbConnection));
        }

        /// <summary>
        /// Add to DI container user implementation of ILibraryRepository
        /// </summary>
        /// <typeparam name="T">Must inherit from ILibraryRepository</typeparam>
        /// <param name="collection"></param>
        public static void AddLibraryRepository<T>(this IServiceCollection collection) where T : class, ILibraryRepository
        {
            collection.AddScoped<ILibraryRepository, T>();
        }

        /// <summary>
        /// Add to DI container user implementation of ILogger
        /// </summary>
        /// <typeparam name="T">Must inherit from ILogger</typeparam>
        /// <param name="collection"></param>
        public static void AddLogger<T>(this IServiceCollection collection) where T : class, ILogger
        {
            collection.AddTransient<ILogger, T>();
        }
    }
}
