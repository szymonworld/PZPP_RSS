using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using PROJEKT_PZPP.RSSParser;
using PROJEKT_PZPP.Services;
using PROJEKT_PZPP.Models;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(PROJEKT_PZPP.Startup))]

namespace PROJEKT_PZPP
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (ChannelsContext con = new ChannelsContext())
            { 
                con.Database.CreateIfNotExists();
            }

            AutofacService service = new AutofacService();
            GlobalConfiguration.Configuration.UseSqlServerStorage("ChannelsContext");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.RemoveIfExists("RSSFeed");
            RecurringJob.AddOrUpdate("RSSFeed",() => ParseRSS(), Cron.MinuteInterval(2));

        }

           public async Task<bool> ParseRSS()
           {
               RSSParser.RSSParser parser = new RSSParser.RSSParser("http://www.rss.lostsite.pl/index.php?rss=6");
               CheckAndAddNews(await parser.Parse());
               return true;
           }

           protected void CheckAndAddNews(List<Channel> news)
           {

            ChannelsContext con = new ChannelsContext();
            RepositoryService<Channel> channels = new RepositoryService<Channel>(con);
            RepositoryService<ChannelItem> channelItems = new RepositoryService<ChannelItem>(con);

            foreach (var item in channelItems.GetAll())
            {
                item.State = false;
                channelItems.Edit(item);
            }

            channelItems.Save();

            foreach (var item in news)
               {
                   bool found = false;

                   foreach (var item2 in channels.GetAll())
                   {
                       if (item2.Link == item.Link)
                           found = true;
                   }

                if (!found)
                    channels.Add(item);
               }
           }
    }
    }
