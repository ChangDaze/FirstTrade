namespace FirstTrade_.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("customer")]
    public partial class customer
    {
        public int id { get; set; }

        public int? Cash { get; set; }

        public int? Position { get; set; }

        public int? Profit { get; set; }

        public int? BuyCost { get; set; }

        public int? Status { get; set; }
    }
}
