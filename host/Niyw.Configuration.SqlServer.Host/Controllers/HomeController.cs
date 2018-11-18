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
        public static string CacheKey1 { get { return "_CacheKey1"; } }
        public IActionResult Contact() {
            var cacheItem1 = _cache.GetOrCreate(CacheKey1, entry => {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(5));
                return GetCacheItemValueFromSqlServer1();
            });
            //GetCacheItemValueFromSqlServer();
            //ViewData[CacheKey1] = _cache.Get<string>(CacheKey1);
            ViewData[CacheKey1] = cacheItem1;
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetCacheItemValueFromSqlServer1() {
            var dbConnStr = _configuration.GetConnectionString("AppConfigDB");
            using (SqlConnection sqlConn = new SqlConnection(dbConnStr)) {
                using (SqlCommand sqlCmd = new SqlCommand("select ItemKey,ItemValue from Tb_Cfg", sqlConn)) {                    
                    sqlConn.Open();
                    using (SqlDataReader sqlReader = sqlCmd.ExecuteReader()) {
                        while (sqlReader.Read()) {
                            return Convert.ToString(sqlReader["ItemValue"]) ?? "db value is null";
                        }
                    }
                }
            }
            return "Cannot read from db";
        }

        private void GetCacheItemValueFromSqlServer() {
            var tarVal = string.Empty;
            try {
                var dbConnStr = _configuration.GetConnectionString("AppConfigDB");
                //SqlDependency.Start(dbConnStr);
                using (SqlConnection sqlConn = new SqlConnection(dbConnStr)) {
                    using (SqlCommand sqlCmd = new SqlCommand("select ItemKey,ItemValue from Tb_Cfg", sqlConn)) {
                        SqlDependency dependency = new SqlDependency(sqlCmd);
                        dependency.OnChange += Dependency_OnChange;
                        sqlConn.Open();
                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader()) {
                            while (sqlReader.Read()) {
                                tarVal = Convert.ToString(sqlReader["ItemValue"]) ?? "db value is null";
                            }
                        }
                    }
                }
                //SqlDependency.Stop(dbConnStr);
            }
            catch (Exception) {
                tarVal = "Cannot read from db";
            }
            _cache.GetOrCreate(CacheKey1, entry => {
                entry.SetPriority(CacheItemPriority.NeverRemove);
                return tarVal;
            });
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e) {
            //throw new NotImplementedException();
            GetCacheItemValueFromSqlServer();
        }
    }
}
