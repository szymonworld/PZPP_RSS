using PROJEKT_PZPP.Models;
using PROJEKT_PZPP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJEKT_PZPP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ChannelsContext con = new ChannelsContext();
            RepositoryService<Channel> channels = new RepositoryService<Channel>(con);
            return View(channels.GetAll().ToList());
        }

        public ActionResult ShowFeed(int id)
        {
            RepositoryService<Channel> feed = new RepositoryService<Channel>(new ChannelsContext());
            List<Channel> channelsList = feed.FindBy(p => (p.Id == id)).ToList();
            ViewBag.ChannelTitle = channelsList.FirstOrDefault().Title;
            return PartialView(channelsList.FirstOrDefault().Items);
        }
    }
}