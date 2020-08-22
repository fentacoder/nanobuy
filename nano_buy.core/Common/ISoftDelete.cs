using System;
using System.Collections.Generic;
using System.Text;

namespace NanoShop.Core.Common
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
