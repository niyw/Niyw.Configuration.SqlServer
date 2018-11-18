using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nyw.Configuration.SqlServer.Host.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace Nyw.Configuration.SqlServer.Host.Controllers {
    public class HomeController : Controller {
        private readonly IConfiguration _configuration = null;
        private readonly IMemoryCache _cache;
        public HomeController(IConfiguration configuration, IMemoryCache memoryCache) {
            this._configuration = configuration;
           this._cache = memoryCache;
        }
        public IActionResult Index() {
            return View();
        }

        public IActionResult About() {
            ViewData["TestDb1"] = _configuration.GetConnectionString("TestDb1");
            ViewData["DefaultConnection"] = _configuration.GetConnectionString("DefaultConnection");
            ViewData["trademark"] = _configuration.GetValue<string>("trademark");
            ViewData["starship:length"] = _configuration.GetValue<float>("starship:length");
            ViewData["starship:commissioned"] = _configuration.GetValue<bool>("starship:commissioned");
            ViewData["section2:subsection0:key0"] = _configuration.GetValue<string>("section2:subsection0:key0");
            return View();
        }

        public IActionResult Contact() {
            var cacheItem1 = _cache.GetOrCreate(CacheKeys.CacheKey1, entry =>
            {
                entry.SetPriority(CacheItemPriority.NeverRemove);
                return "Hello world";
            });
            ViewData[CacheKeys.CacheKey1] = cacheItem1;
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public static class CacheKeys {
        public static string CacheKey1 { get { return "_CacheKey1"; } }
    }
}
