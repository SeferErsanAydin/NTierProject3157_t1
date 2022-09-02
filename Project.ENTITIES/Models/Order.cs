using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class Order : BaseEntity
    {
        public string ShippedAddress { get; set; }
        public decimal TotalPrice { get; set; } //kolaylık saglaması için bu property buraya açıldı
        public string UserName { get; set; }
        public string Email { get; set; }

        public int? AppUserID { get; set; }
        public int? ShipperID { get; set; }
        //relational properties

        public virtual AppUser AppUser { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual Shipper Shipper { get; set; }
    }
}
