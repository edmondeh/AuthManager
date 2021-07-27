using System;

namespace AuthManager.Application.Interfaces.Shared
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        public string Username { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }
        public DateTime CreatedOn { get; }

    }
}