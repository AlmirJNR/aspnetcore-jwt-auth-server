using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class AppUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
