using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class Payment
    {
        public string PayId { get; set; } = null!;
        public string? ContractId { get; set; }
        public string DatePayed { get; set; } = null!;
        public float AmountPayed { get; set; }

        public virtual Contract? Contract { get; set; }
    }
}
