using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class Apartment
    {
        public Apartment()
        {
            Contracts = new HashSet<Contract>();
            ElectricityContracts = new HashSet<ElectricityContract>();
            WaterContracts = new HashSet<WaterContract>();
        }

        public string AptId { get; set; } = null!;
        public string AptNum { get; set; } = null!;
        public byte Capacity { get; set; }
        public float MonthlyCost { get; set; }
        public byte Bedrooms { get; set; }
        public bool HasKitchen { get; set; }
        public byte Bathrooms { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<ElectricityContract> ElectricityContracts { get; set; }
        public virtual ICollection<WaterContract> WaterContracts { get; set; }
    }
}
