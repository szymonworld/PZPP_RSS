using HtmlAgilityPack;
using PROJEKT_PZPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace PROJEKT_PZPP.RSSParser
{
    public class RSSParser
    {
        private string _website;

        public RSSParser(string website)
        {
            _website = website;
        }

        public async Task<List<Channel>> Parse()
        {
            List<Channel> channels = await GetChannels();
           
            foreach (var item in channels)
            {
                List<ChannelItem> channelItems = new List<ChannelItem>();

                try
                {
                    XmlDocument rssXmlDoc = new XmlDocument();
                    rssXmlDoc.Load(item.Link);

                    XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");
                    StringBuilder rssContent = new StringBuilder();

                    foreach (XmlNode rssNode in rssNodes)
                    {
                        XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                        string title = rssSubNode != null ? rssSubNode.InnerText : "";
                        rssSubNode = rssNode.SelectSingleNode("link");
                        string link = rssSubNode != null ? rssSubNode.InnerText : "";
                        rssSubNode = rssNode.SelectSingleNode("description");
                        string description = rssSubNode != null ? rssSubNode.InnerText : "";
                        rssSubNode = rssNode.SelectSingleNode("pubDate");
                        string pubDate = rssSubNode != null ? rssSubNode.InnerText : "";

                        channelItems.Add(new ChannelItem { Title = title, Description = Regex.Replace(description, @"<[^>]*>|&#[^>]*;", String.Empty), Link = link, PubDate = pubDate, State = true });
                    }

                }
                catch (Exception)
                {
                   // channelItems.Add(new ChannelItem { PubDate = "null", Title = "null", Description = "null", Link = "null" });
                }

                item.Items = channelItems;
            }

            return channels;
        }


        protected async Task<List<Channel>> GetChannels()
        {
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(_website);
            string source = Encoding.GetEncoding("iso-8859-2").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);

            List<Channel> links = new List<Channel>();

            foreach (var item in resultat.DocumentNode.SelectNodes("//div[@id='panel_R']"))
            {
                foreach (var item2 in item.Descendants("a").Where(x => x.Name == "a" && x.Attributes["rel"] != null && x.Attributes["rel"].Value.Contains("nofollow")).ToList().Select(x => x.Attributes["href"].Value).ToList())
                {
                    try
                    {
                        XmlDocument rssXmlDoc = new XmlDocument();
                        rssXmlDoc.Load(item2);
                        XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel");

                        StringBuilder rssContent = new StringBuilder();
                        string description = "";
                        string title = "";
                        string link = "";

  
                        XmlNode rssSubNode = rssNodes[0].SelectSingleNode("title");
                        title = rssSubNode != null ? rssSubNode.InnerText : "";

                        rssSubNode = rssNodes[0].SelectSingleNode("link");
                        link = rssSubNode != null ? rssSubNode.InnerText : "";

                        rssSubNode = rssNodes[0].SelectSingleNode("description");
                        description = rssSubNode != null ? rssSubNode.InnerText : "";

                        links.Add(new Channel { Link = item2, Title = title, Description = description });
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return links;
        }


    }
}
