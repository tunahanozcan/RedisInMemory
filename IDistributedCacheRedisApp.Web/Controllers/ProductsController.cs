using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            
            DistributedCacheEntryOptions cacheOptions = new();
            cacheOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            Product prod = new() { Price = 12, Name = "Urun1" ,Id=1};
            string jsonProduct = JsonSerializer.Serialize(prod);

            await _distributedCache.SetStringAsync("product:1", jsonProduct,cacheOptions);

            //_distributedCache.SetString("name2", "Tunahan", cacheOptions);
            //await _distributedCache.SetStringAsync("surname", "Ozcan", cacheOptions);

            return View();
        }
        public IActionResult Show()
        {
            string name = _distributedCache.GetString("surname");

            var jsonProd = _distributedCache.GetString("product:1");
            var prod = JsonSerializer.Deserialize<Product>(jsonProd);
            ViewBag.name = prod.Name;
            return View();
        }
        public IActionResult Remove()
        {
            _distributedCache.Remove("name2");
            return View();
        }
        public IActionResult ImageUrl()
        {
            byte[] resimByte = _distributedCache.Get("resim");

            return File(resimByte,"image/png");
        }
        public IActionResult ImageCache()
        {
            //görsel byte a dönüşecek
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/error2.png");
            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("resim", imageByte);

            return View();
        }
    }
}
