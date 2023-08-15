using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class ElectricityContract
    {
        public string ElecContractId { get; set; } = null!;
        public string? AptId { get; set; }
        public string ServiceNum { get; set; } = null!;
        public string MeasurerNum { get; set; } = null!;
        public string Rmu { get; set; } = null!;
        public string PaymentDue { get; set; } = null!;
        public string ShutOffDate { get; set; } = null!;

        public virtual Apartment? Apt { get; set; }
    }
}
