using Colosseo.Exercises.Ticketing.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Ticketing.Api.v1.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing.Api.v1.Services;
using Ticketing.Dto.Infrastructure;

namespace Ticketing.Api.v1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IFactory<TicketingDbContext> factoryContext = Configuration.CreateDbFactory();
#if DEBUG
            services.AddSingleton<IEventsService>(new EventsService(factoryContext));
            services.AddSingleton<ITicketService>(new TicketService(factoryContext));
#elif SomeTestConfiguration_1
#elif SomeTestConfiguration_2
#elif SomeTestConfiguration_3
#endif
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
