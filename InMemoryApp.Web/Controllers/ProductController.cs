using System;
using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if(!_memoryCache.TryGetValue("zaman", out string zamanCache))
            {
                MemoryCacheEntryOptions options=new();
                options.AbsoluteExpiration=DateTime.Now.AddMinutes(1);
                options.SlidingExpiration=TimeSpan.FromMinutes(10);
                options.Priority=CacheItemPriority.High;



                options.RegisterPostEvictionCallback((key,value,reason,state)=>{
                    _memoryCache.Set("callback",$"{key}->{value} = sebep: {reason},state: {state}, options: {options}");

                });
                _memoryCache.Set("zaman", DateTime.Now.ToString(),options);
            }


            Product prod=new(){
                Id=1,
                Name="Laptop",
                Price=5000
            };
            _memoryCache.Set<Product>("product:1",prod);



            return View();
        }
        public IActionResult Show()
        {
            _memoryCache.TryGetValue("zaman", out string zamanCache);
            _memoryCache.TryGetValue("callback", out string callbackCache);
            ViewBag.zaman= zamanCache;
            ViewBag.callback= callbackCache;

            ViewBag.product=_memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
