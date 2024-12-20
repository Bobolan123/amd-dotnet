using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache _cache;

        public HomeController(ApplicationDbContext _db, IMemoryCache cache)
        {
            db = _db;
            _cache = cache;
        }

        public IActionResult Index(UrlManager obj, int val = 0)
        {
            return View(obj);
        }

        // POST : Create URL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UrlManager obj)
        {
            if (string.IsNullOrEmpty(obj.Url) || !Uri.TryCreate(obj.Url, UriKind.Absolute, out var inputUrl))
                return View(obj);

            const string chars = "ABCEDFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890@$&";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 8)
                .Select(x => x[random.Next(x.Length)])
                .ToArray()
            );

            while (db.Urls.Any(u => u.ShortUrl == randomString))
            {
                randomString = new string(Enumerable.Repeat(chars, 8)
                    .Select(x => x[random.Next(x.Length)])
                    .ToArray()
                );
            }

            var urlObj = new UrlManager()
            {
                Url = obj.Url,
                ShortUrl = randomString
            };

            db.Urls.Add(urlObj);
            db.SaveChanges();

            var ctx = HttpContext;

            urlObj = new UrlManager()
            {
                Url = obj.Url,
<<<<<<< HEAD
                ShortUrl = (obj.Url.Length > 50) ?
                $"{ctx.Request.Scheme}://{ctx.Request.Host.Host}:{ctx.Request.Host.Port}/SUrl/{randomString}" :
                obj.Url
=======
<<<<<<< HEAD
                ShortUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host.Host}:{ctx.Request.Host.Port}/SUrl/{randomString}"
=======
                ShortUrl = (obj.Url.Length > 50)? $"{ctx.Request.Scheme}://{ctx.Request.Host.Host}:{ctx.Request.Host.Port}/SUrl/{randomString}" : obj.Url
                /*ShortUrl = $"localhost:44313/SUrl/{randomString}"*/
>>>>>>> 1be0a35b7e9847b1f6577e7ec39ff9cb453162cc
>>>>>>> c65fc58d1dcb4d515b493bebc233b44fcec81261
            };

            return RedirectToAction("Index", urlObj);
        }

        public IActionResult RedirectToOriginalUrl(string shortUrl)
        {
            // Check if URL is in cache
            if (!_cache.TryGetValue(shortUrl, out string originalUrl))
            {
                // If not in cache, fetch from database
                var urlObj = db.Urls.FirstOrDefault(u => u.ShortUrl == shortUrl);

                if (urlObj == null)
                {
                    return NotFound();
                }

                originalUrl = urlObj.Url;

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Cache entry expires if not accessed for 30 minutes

                // Save data in cache
                _cache.Set(shortUrl, originalUrl, cacheEntryOptions);
            }

            return Redirect(originalUrl);
<<<<<<< HEAD
        }
        public IActionResult ManageUrls()
        {
            var urls = db.Urls.ToList();
            return View(urls);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUrl(int id)
        {
            var url = db.Urls.Find(id);
            if (url != null) {
                db.Urls.Remove(url);
                db.SaveChanges();
            }
            return RedirectToAction("ManageUrls");
=======
>>>>>>> c65fc58d1dcb4d515b493bebc233b44fcec81261
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
