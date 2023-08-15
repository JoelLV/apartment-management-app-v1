using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class Renter
    {
        public Renter()
        {
            Contracts = new HashSet<Contract>();
        }

        public string RenterId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
