using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Data.Data
{
    public class EsvccorpdbContext : DbContext
    {
        public EsvccorpdbContext()
            : base("EsvccorpdbContext")
        {


        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
