using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class WaterContract
    {
        public string WaterContractId { get; set; } = null!;
        public string? AptId { get; set; }
        public string AccountNum { get; set; } = null!;
        public string ConsumeStartDate { get; set; } = null!;
        public string ConsumeEndDate { get; set; } = null!;
        public string ExpDate { get; set; } = null!;

        public virtual Apartment? Apt { get; set; }
    }
}
