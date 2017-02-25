using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace PZPP_RSS_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<RSSLinks> _rssLinks;
        private List<RSSChannels> _rssChannels;
        public MainWindow()
        {
            InitializeComponent();
            _rssLinks = new List<RSSLinks>();
            _rssChannels = new List<RSSChannels>();
            string htmlstring = "http://www.rss.lostsite.pl/index.php?rss=6";
            Parsing(htmlstring, _rssLinks);
            DisplayRSSList(_rssLinks);
        }
        private async void Parsing(string website, List<RSSLinks> links)
        {
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(website);
            string source = Encoding.GetEncoding("iso-8859-2").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);
            foreach (var item in resultat.DocumentNode.SelectNodes("//div[@id='panel_R']"))
            {
                foreach (var item2 in item.Descendants("a").Where(x => x.Name == "a" && x.Attributes["rel"] != null && x.Attributes["rel"].Value.Contains("nofollow")).ToList().Select(x => x.Attributes["href"].Value).ToList())
                {
                    links.Add(new RSSLinks { Link = item2 });
                }
            }
            //DisplayRSSList(links);
            AddToListView(links);
        }
        private void DisplayRSSList(List<RSSLinks> links)
        {
            foreach (var item in links)
            {
                textBox.Text += item.Link;
                textBox.Text += Environment.NewLine;
            }
        }

        private void AddToListView(List<RSSLinks> links)
        {
            foreach (var item in links)
            {
                listView.Items.Add(item.Link);
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string url = Convert.ToString(listView.SelectedItems[0]);
            textBox.Text = url;
            ParseWebsite(url);
        }

        private async void ParseWebsite(string website)
        {
            _rssChannels = new List<RSSChannels>();
            try
            {
                XmlDocument rssXmlDoc = new XmlDocument();
                rssXmlDoc.Load(website);

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
                    _rssChannels.Add(new RSSChannels { Title = title, Description = description, Link = link });
                    rssContent.Append(Environment.NewLine + "Tytuł: " + title + Environment.NewLine + "Opis: " + description + Environment.NewLine + "Źródło: " + link);
                }
                textBox.Text = rssContent.ToString();
            }
            catch (Exception)
            {
                textBox.Text += "     <=== Brak kanału RSS :(";
                MessageBox.Show("Błąd, brak kanału RSS");
               
            }
        }
    }
}
