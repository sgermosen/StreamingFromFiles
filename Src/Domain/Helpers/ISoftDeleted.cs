﻿namespace Domain.Helpers
{
    public interface ISoftDeleted
    {
        bool Deleted { get; set; }
    }
}
