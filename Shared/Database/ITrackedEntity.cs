using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniars.Shared.Database
{
    public interface ITrackedEntity
    {
        DateTime? CreatedAt { get; set; }

        DateTime? UpdatedAt { get; set; }
    }
}
