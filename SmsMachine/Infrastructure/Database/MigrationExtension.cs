using Microsoft.EntityFrameworkCore;
using SmsMachine.Infrastructure.Data;

namespace SmsMachine.Api.Infrastructure.Database
{
    public static class MigrationExtension
    {
        public static IApplicationBuilder InizializeDatabase(this IApplicationBuilder app)

        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SmsDbContext>();

            dbContext.Database.Migrate();

            return app;
        }
    }
}
