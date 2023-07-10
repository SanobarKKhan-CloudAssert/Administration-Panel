// using System;
// using CacheManager.Core;
// using MateMachine.Chat.Api;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace MateMachine.AdminPanel.Api;
//
// public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
// {
//     private readonly ILogger<DataContext> _logger;
//
//     public DataContextFactory()
//     {
//         _logger = new NullLogger<DataContext>();
//     }
//
//     public DataContext CreateDbContext(
//         string[] args
//     )
//     {
//         var configBuilder = new ConfigurationBuilder();
//
//         configBuilder.SetBasePath(System.IO.Directory.GetCurrentDirectory());
//         configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
//         configBuilder.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
//         configBuilder.AddEnvironmentVariables();
//
//         var config = configBuilder.Build();
//
//         var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
//         optionsBuilder.ConfigAppDb(config.GetSection(nameof(DbOptions)).Get<DbOptions>()!);
//
//         return new DataContext(optionsBuilder.Options, () => DateTime.Now);
//     }
// }
