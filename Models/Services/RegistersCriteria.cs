using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstTrade_.Models.Services
{
    public class RegistersCriteria
    {
        public int Name { get; set; }
        public int Total { get; set; }
    }
    public class StockVM2
    {
        public int count { get; set; }
        public int id { get; set; }
        public string dev { get; set; }
        public List<double?> price { get; set; }
        public double? UpL { get; set; }
        public double? DownL { get; set; }

    }
}