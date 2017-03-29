using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKT_PZPP.Models
{
    public class Channel : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public virtual List<ChannelItem> Items { get; set; }
    }
}
