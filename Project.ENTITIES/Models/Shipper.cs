using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class Shipper : BaseEntity
    {
        public string CompantName { get; set; }
        public string Phone { get; set; }
        //relational properties
        public virtual List<Order> Orders { get; set; }
    }
}
