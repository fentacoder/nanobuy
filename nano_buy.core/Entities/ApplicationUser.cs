using Microsoft.AspNetCore.Identity;
using NanoShop.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoShop.Core.Entities
{
    public class ApplicationUser : IdentityUser<int>, AuditProperties, ISoftDelete
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailConfirmationLink { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}
