namespace FirstTrade_.Models.EFModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<customer> customers { get; set; }
        public virtual DbSet<stockprice> stockprices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<stockprice>()
                .Property(e => e.年月日)
                .IsUnicode(false);
        }
    }
}
