using AuthManager.Application.Interfaces.Shared;
using System;

namespace AuthManager.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}