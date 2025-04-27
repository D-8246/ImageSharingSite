using HW4._21.Data;
using HW4._21.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace HW4._21.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private static string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=ImagesSite;Integrated Security=True;Trust Server Certificate=true;";
        public ImageDB db = new(_connectionString);

        public HomeController (IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            var Image = new Image
            {
                Name = image.FileName,
                Location = fileName,
                Password = password,
            };
            int Id = db.Add(Image);
            SubmitViewModel svm = new SubmitViewModel
            {
                Password = password,
                Id = Id,
            };

            return View(svm);
        }

        public IActionResult ViewImage(int id)
        {
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }       
            var image = db.GetImageById(id);

            var vivm = new ViewImageViewModel
            {
                Image = image,
            };

            if (ids.Contains(image.Id))
            {
                vivm.IsAuthroized = true;
            }  
            
            return View(vivm);
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
                HttpContext.Session.Set("ids", ids);
            }
            var image = db.GetImageById(id);
            if((!ids.Contains(image.Id)) && image.Password == password)
            {
                ids.Add(image.Id);
                HttpContext.Session.Set("ids", ids);
            }

            else
            {
                return View(new ViewImageViewModel
                {
                    Image = image,
                    IsAuthroized = false,
                    Meessage = "Incorrect Password, try again.",
                });
            }

            return RedirectToAction("ViewImage", new { id = id });
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
