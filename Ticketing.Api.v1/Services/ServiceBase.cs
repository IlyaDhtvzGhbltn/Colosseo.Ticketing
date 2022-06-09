using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing.Dto.Infrastructure;

namespace Ticketing.Api.v1.Services
{
    public class ServiceBase
    {
        protected readonly IFactory<TicketingDbContext> _factoryDb;

        public ServiceBase(IFactory<TicketingDbContext> factoryDb)
        {
            _factoryDb = factoryDb;
        }
    }
}
