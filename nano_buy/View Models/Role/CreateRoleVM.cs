using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NanoShop.Web.VM.Role
{
    public class CreateRoleVM
    {
        [Required]
        public string RoleName { get; set; }
    }
}
