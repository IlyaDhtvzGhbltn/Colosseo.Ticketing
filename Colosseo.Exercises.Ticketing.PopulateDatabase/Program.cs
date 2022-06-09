using Colosseo.Exercises.Ticketing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Colosseo.Exercises.Ticketing.PopulateDatabase
{
  class Program
  {
    static void Main(string[] args)
    {
      using IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(ConfigureServices)
        .Build();

      host.RunAsync().Wait();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
      var connectionString = context.Configuration["ConnectionStrings:DefaultConnection"];

      services.AddDbContext<TicketingDbContext>(options =>
        options.UseSqlServer(connectionString), ServiceLifetime.Transient);

      services.AddHostedService<DatabaseTester>();
    }
  }
}
