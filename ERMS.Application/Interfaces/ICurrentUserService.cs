using System;

namespace ERMS.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
