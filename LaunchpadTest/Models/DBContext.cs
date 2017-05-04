using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LaunchpadTest.Models {
    public class DBContext : DbContext {
        public DbSet<UserAccount> userAccount { get; set; }
    }
}