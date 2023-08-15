using System;
using System.Collections.Generic;

namespace Apt_Management_App.Database
{
    public partial class Contract
    {
        public Contract()
        {
            Payments = new HashSet<Payment>();
        }

        public string ContractId { get; set; } = null!;
        public string? AptId { get; set; }
        public string? RenterId { get; set; }
        public string ExpDate { get; set; } = null!;
        public byte PaymentDay { get; set; }
        public string DateSigned { get; set; } = null!;
        public float Deposit { get; set; }
        public byte NumResidents { get; set; }

        public virtual Apartment? Apt { get; set; }
        public virtual Renter? Renter { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
