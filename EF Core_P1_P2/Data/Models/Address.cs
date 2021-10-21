using System;
using System.Collections.Generic;

#nullable disable

namespace EF_Core_P1_P2.Data.Models
{
    public partial class Address
    {
        public Address()
        {
            Employees = new HashSet<Employee>();
        }

        public Address(string addressText, int townId)
        {
            AddressText = addressText;
            TownId = townId;
        }
        public int AddressId { get; set; }
        public string AddressText { get; set; }
        public int? TownId { get; set; }

        public virtual Town Town { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
