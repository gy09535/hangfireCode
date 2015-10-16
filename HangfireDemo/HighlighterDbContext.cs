using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangfireDemo.Models;

namespace HangfireDemo
{
    public class HighlighterDbContext : DbContext
    {
        public HighlighterDbContext()
            : base("HighlighterDb")
        {

        }
        public DbSet<ConfigManagers> ConfigManager { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }
    }
}
