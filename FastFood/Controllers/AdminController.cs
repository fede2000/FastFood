using FastFood.Models;
using FastFood.Rules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FastFood.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category()
        {
            ViewBag.Title = "Category";
            ViewBag.Page = "Category";
           
            var rule = new CategoryRule(_configuration);
            var categories = rule.GetCategories();
            var viewModel = new CategoryViewModel
            {
                Categories = categories,
                NewCategory = new Category()
            };
			return View(viewModel); 
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
				var model = viewModel.NewCategory;
				if (model.ImageFile != null)
                {
                    // Generar una ruta única para el archivo
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, newFileName);

                    // Guardar el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualizar la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }
				model.CreateDate = DateTime.Now;

				var rule = new CategoryRule(_configuration);
                rule.InsertCategory(model);

                return RedirectToAction("Category");
            }
			else
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors);
				foreach (var error in errors)
				{
					Console.WriteLine(error.ErrorMessage);
				}
			}
			var rule1 = new CategoryRule(_configuration);
			viewModel.Categories = rule1.GetCategories() ?? new List<Category>();
			return View("Category", viewModel); // Si no es válido, retornar a la vista con el modelo
        }
    }
}