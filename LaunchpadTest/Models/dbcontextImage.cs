using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LaunchpadTest.Models {
    public class dbcontextImage : DbContext {
        public DbSet<ImageModel> imageModel { get; set; }
    }
}