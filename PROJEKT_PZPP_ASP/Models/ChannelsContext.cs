using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKT_PZPP.Models
{
    public class ChannelsContext : DbContext
    {
        public ChannelsContext()
        : base("name=ChannelsContext")
        {
            Database.SetInitializer<ChannelsContext>(new CreateDatabaseIfNotExists<ChannelsContext>());
        }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelItem> ChannelItems { get; set; }

    }
}
