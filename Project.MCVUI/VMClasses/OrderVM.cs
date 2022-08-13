using Project.ENTITIES.Models;
using Project.MCVUI.ConsumerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MCVUI.VMClasses
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public List<Order> Orders { get; set; }
        public PaymentDTO PaymentDTO { get; set; }
    }
}