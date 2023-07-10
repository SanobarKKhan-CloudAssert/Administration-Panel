// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace MateMachine.AdminPanel.Api;
//
// public class DbMigrator
// {
//     private readonly IServiceScopeFactory _serviceScopeFactory;
//
//     public DbMigrator(IServiceScopeFactory serviceScopeFactory)
//     {
//         _serviceScopeFactory = serviceScopeFactory;
//     }
//
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         await using var scope = _serviceScopeFactory.CreateAsyncScope();
//         var context = scope.ServiceProvider.GetRequiredService<DataContext>();
//
//         await context.Database.MigrateAsync(cancellationToken);
//     }
//
//     public Task StopAsync(CancellationToken cancellationToken)
//     {
//         return Task.CompletedTask;
//     }
// }
