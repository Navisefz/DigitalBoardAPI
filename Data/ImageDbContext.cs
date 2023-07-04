using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TV_DASH_API.Models;

namespace TV_DASH_API.Models
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options)
        {

        }
        public DbSet<CebuImageModel> TVDash_CebuImages { get; set; }
        public DbSet<ClarkImageModel> TVDash_ClarkImages { get; set; }
    }
}
