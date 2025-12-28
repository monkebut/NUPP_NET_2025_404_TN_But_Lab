using System;

namespace CinemaManagement.Common
{
    /// <summary>
    /// Interface for entities that have a Guid identifier
    /// </summary>
    public interface IHasId
    {
        Guid Id { get; set; }
    }
}


