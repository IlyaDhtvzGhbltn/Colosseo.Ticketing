using Colosseo.Exercises.Ticketing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing.Dto.Infrastructure;

namespace Ticketing.Api.v1.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IFactory<TicketingDbContext> CreateDbFactory(this IConfiguration configuration) 
        {
            return new DbFactory<TicketingDbContext>(() =>
            {
                var optBuilder = new DbContextOptionsBuilder<TicketingDbContext>();
                optBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
                optBuilder.EnableSensitiveDataLogging();
                return new TicketingDbContext(optBuilder.Options);
            });
        }
    }
}
