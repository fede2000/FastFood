using FastFood.Models;
using FastFood.Models.Category;
using FastFood.Models.Contact;
using FastFood.Models.Product;
using FastFood.Models.SellingReport;
using FastFood.Rules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FastFood.Controllers
{
    // CONTROLADOR DE LA PAGINA DEL ADMINISTRADOR
    public class AdminController : Controller
    {
        // Campos privados de solo lectura para almacenar las dependencias inyectadas
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor del controlador que recibe las dependencias mediante inyección de dependencias
        public AdminController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration; 
            _webHostEnvironment = webHostEnvironment; 
        }

        // Acción para manejar la solicitud GET a la página de inicio del administrador
        public IActionResult Index()
        {
            // Establece el título de la página en ViewBag para usarlo en la vista
            ViewBag.Title = "Dashboard";
            // Establece la página actual en ViewBag para usarla en la vista
            ViewBag.Page = "Dashboard";

            // Crea una instancia de DashboardRule pasando la configuración
            var rule = new DashboardRule(_configuration);
            // Obtiene los datos del tablero de instrumentos llamando al método GetDashboardData de DashboardRule
            var dashboardData = rule.GetDashboardData();

            // Crea un objeto DashboardViewModel y lo llena con los datos obtenidos
            var viewModel = new DashboardViewModel
            {
                TotalCategories = dashboardData.TotalCategories, // Asigna el número total de categorías
                TotalProducts = dashboardData.TotalProducts, // Asigna el número total de productos
                TotalOrders = dashboardData.TotalOrders, // Asigna el número total de órdenes
                DeliveredOrders = dashboardData.DeliveredOrders, // Asigna el número de órdenes entregadas
                PendingOrders = dashboardData.PendingOrders, // Asigna el número de órdenes pendientes
                TotalUsers = dashboardData.TotalUsers, // Asigna el número total de usuarios
                TotalSold = dashboardData.TotalSold, // Asigna el total vendido
                TotalFeedbacks = dashboardData.TotalFeedbacks // Asigna el número total de comentarios
            };

            // Devuelve la vista correspondiente a la acción Index con el modelo viewModel
            return View(viewModel);
        }


        // Acción para manejar la solicitud GET a la página de categorías
        public IActionResult Category()
        {
            ViewBag.Title = "Category"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Category"; // Establece la página actual en ViewBag

            var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            var categories = rule.GetCategories(); // Obtiene las categorías llamando al método GetCategories de CategoryRule
            var viewModel = new CategoryViewModel
            {
                Categories = categories, // Asigna las categorías obtenidas al modelo de vista
                NewCategory = new Category() // Inicializa una nueva categoría
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción Category con el modelo viewModel
        }

        // Acción para manejar la solicitud POST para agregar una nueva categoría
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid) // Verifica si el modelo es válido
            {
                var model = viewModel.NewCategory; // Obtiene la nueva categoría del modelo de vista
                if (model.ImageFile != null) // Verifica si se ha subido un archivo de imagen
                {
                    // Genera una ruta única para el archivo
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath)) // Verifica si el directorio de subida no existe
                    {
                        Directory.CreateDirectory(uploadPath); // Crea el directorio de subida
                    }

                    var filePath = Path.Combine(uploadPath, newFileName); // Combina la ruta del directorio con el nuevo nombre del archivo

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }
                model.CreatedDate = DateTime.Now; // Establece la fecha de creación de la categoría

                var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
                rule.InsertCategory(model); // Inserta la nueva categoría llamando al método InsertCategory de CategoryRule

                TempData["SuccessMessage"] = "Categoria Agregada Correctamente"; // Establece un mensaje de éxito en TempData

                return RedirectToAction("Category"); // Redirige a la acción Category
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors); // Obtiene los errores del modelo
                foreach (var error in errors) // Imprime los mensajes de error en la consola
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var rule1 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            viewModel.Categories = rule1.GetCategories() ?? new List<Category>(); // Obtiene las categorías y las asigna al modelo de vista
            return View("Category", viewModel); // Si el modelo no es válido, retorna a la vista con el modelo de vista actualizado
        }

        // Acción para mostrar el formulario de edición de una categoría
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            ViewBag.Title = "EditarCategoria"; // Establece el título de la página en ViewBag
            ViewBag.Page = "EditarCategoria"; // Establece la página actual en ViewBag
            var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            var category = rule.GetCategoryById(id); // Obtiene la categoría por ID llamando al método GetCategoryById de CategoryRule
            if (category == null) // Verifica si la categoría no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }
            var viewModel = new CategoryViewModel
            {
                NewCategory = category, // Asigna la categoría obtenida al modelo de vista
                Categories = rule.GetCategories() // Obtiene todas las categorías y las asigna al modelo de vista
            };
            return View(viewModel); // Devuelve la vista correspondiente a la acción EditCategory con el modelo viewModel
        }

        // Acción para procesar el formulario de edición de una categoría
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid) // Verifica si el modelo es válido
            {
                var model = viewModel.NewCategory; // Obtiene la categoría del modelo de vista
                if (model.ImageFile != null) // Verifica si se ha subido un archivo de imagen
                {
                    // Genera una ruta única para el archivo
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath)) // Verifica si el directorio de subida no existe
                    {
                        Directory.CreateDirectory(uploadPath); // Crea el directorio de subida
                    }

                    var filePath = Path.Combine(uploadPath, newFileName); // Combina la ruta del directorio con el nuevo nombre del archivo

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }

                var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
                rule.UpdateCategory(model); // Actualiza la categoría llamando al método UpdateCategory de CategoryRule

                return RedirectToAction("Category"); // Redirige a la acción Category
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors); // Obtiene los errores del modelo
                foreach (var error in errors) // Imprime los mensajes de error en la consola
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var rule1 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            viewModel.Categories = rule1.GetCategories() ?? new List<Category>(); // Obtiene las categorías y las asigna al modelo de vista
            return View(viewModel); // Si el modelo no es válido, retorna a la vista con el modelo de vista actualizado
        }

        // Acción para mostrar la confirmación de eliminación de una categoría
        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            ViewBag.Title = "EliminarCategoria"; // Establece el título de la página en ViewBag
            ViewBag.Page = "EliminarCategoria"; // Establece la página actual en ViewBag
            var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            var category = rule.GetCategoryById(id); // Obtiene la categoría por ID llamando al método GetCategoryById de CategoryRule
            if (category == null) // Verifica si la categoría no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }
            var viewModel = new CategoryViewModel
            {
                NewCategory = category, // Asigna la categoría obtenida al modelo de vista
                Categories = rule.GetCategories() // Obtiene todas las categorías y las asigna al modelo de vista
            };
            return View(viewModel); // Devuelve la vista correspondiente a la acción DeleteCategory con el modelo viewModel
        }

        // Acción para procesar la eliminación de una categoría
        [HttpPost, ActionName("DeleteCategory")]
        public IActionResult DeleteConfirmed(int id)
        {
            var rule = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            rule.DeleteCategory(id); // Elimina la categoría llamando al método DeleteCategory de CategoryRule
            return RedirectToAction("Category"); // Redirige a la acción Category
        }
        // Acción para manejar la solicitud GET a la página de productos
        public IActionResult Product()
        {
            ViewBag.Title = "Productos"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Productos"; // Establece la página actual en ViewBag

            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            var products = rule.GetProducts(); // Obtiene los productos llamando al método GetProducts de ProductRule

            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            var categories = rule2.GetCategories(); // Obtiene las categorías llamando al método GetCategories de CategoryRule

            // Asocia cada producto con su nombre de categoría
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                if (category != null)
                {
                    product.CategoryName = category.Name; // Asigna el nombre de la categoría al producto
                }
            }

            var viewModel = new ProductViewModel
            {
                Products = products, // Asigna los productos obtenidos al modelo de vista
                NewProduct = new Product(), // Inicializa un nuevo producto
                Categories = categories // Asigna las categorías obtenidas al modelo de vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción Product con el modelo viewModel
        }

        // Acción para manejar la solicitud POST para agregar un nuevo producto
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel viewModel)
        {
            if (ModelState.IsValid) // Verifica si el modelo es válido
            {
                var model = viewModel.NewProduct; // Obtiene el nuevo producto del modelo de vista
                if (model.ImageFile != null) // Verifica si se ha subido un archivo de imagen
                {
                    // Genera una ruta única para el archivo
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath)) // Verifica si el directorio de subida no existe
                    {
                        Directory.CreateDirectory(uploadPath); // Crea el directorio de subida
                    }

                    var filePath = Path.Combine(uploadPath, newFileName); // Combina la ruta del directorio con el nuevo nombre del archivo

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }
                model.CreatedDate = DateTime.Now; // Establece la fecha de creación del producto

                var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
                rule.InsertProduct(model); // Inserta el nuevo producto llamando al método InsertProduct de ProductRule

                TempData["SuccessMessage"] = "Producto Agregado Correctamente"; // Establece un mensaje de éxito en TempData

                return RedirectToAction("Product"); // Redirige a la acción Product
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors); // Obtiene los errores del modelo
                foreach (var error in errors) // Imprime los mensajes de error en la consola
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var rule1 = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            viewModel.Products = rule1.GetProducts() ?? new List<Product>(); // Obtiene los productos y los asigna al modelo de vista
            return View("Product", viewModel); // Si el modelo no es válido, retorna a la vista con el modelo de vista actualizado
        }

        // Acción para mostrar la confirmación de eliminación de un producto
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            ViewBag.Title = "EliminarProducto"; // Establece el título de la página en ViewBag
            ViewBag.Page = "EliminarProducto"; // Establece la página actual en ViewBag
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            var product = rule.GetProductById(id); // Obtiene el producto por ID llamando al método GetProductById de ProductRule
            if (product == null) // Verifica si el producto no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }
            var viewModel = new ProductViewModel
            {
                NewProduct = product, // Asigna el producto obtenido al modelo de vista
                Products = rule.GetProducts() // Obtiene todos los productos y los asigna al modelo de vista
            };
            return View(viewModel); // Devuelve la vista correspondiente a la acción DeleteProduct con el modelo viewModel
        }

        // Acción para procesar la eliminación de un producto
        [HttpPost, ActionName("DeleteProduct")]
        public IActionResult DeleteConfirmedProduct(int id)
        {
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            rule.DeleteProduct(id); // Elimina el producto llamando al método DeleteProduct de ProductRule
            return RedirectToAction("Product"); // Redirige a la acción Product
        }

        // Acción para mostrar el formulario de edición de un producto
        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            ViewBag.Title = "ActualizarProducto"; // Establece el título de la página en ViewBag
            ViewBag.Page = "ActualizarProducto"; // Establece la página actual en ViewBag
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            var product = rule.GetProductById(id); // Obtiene el producto por ID llamando al método GetProductById de ProductRule
            if (product == null) // Verifica si el producto no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }

            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule pasando la configuración
            var categories = rule2.GetCategories(); // Obtiene las categorías llamando al método GetCategories de CategoryRule
            var viewModel = new ProductViewModel
            {
                NewProduct = product, // Asigna el producto obtenido al modelo de vista
                Products = rule.GetProducts(), // Obtiene todos los productos y los asigna al modelo de vista
                Categories = categories // Asigna las categorías obtenidas al modelo de vista
            };
            return View(viewModel); // Devuelve la vista correspondiente a la acción UpdateProduct con el modelo viewModel
        }

        // Acción para procesar el formulario de edición de un producto
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel viewModel)
        {
            ViewBag.Title = "ActualizarProducto"; // Establece el título de la página en ViewBag
            ViewBag.Page = "ActualizarProducto"; // Establece la página actual en ViewBag
            if (ModelState.IsValid) // Verifica si el modelo es válido
            {
                var model = viewModel.NewProduct; // Obtiene el producto del modelo de vista
                if (model.ImageFile != null) // Verifica si se ha subido un archivo de imagen
                {
                    // Genera una ruta única para el archivo
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath)) // Verifica si el directorio de subida no existe
                    {
                        Directory.CreateDirectory(uploadPath); // Crea el directorio de subida
                    }

                    var filePath = Path.Combine(uploadPath, newFileName); // Combina la ruta del directorio con el nuevo nombre del archivo

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }

                var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
                rule.UpdateProduct(model); // Actualiza el producto llamando al método UpdateProduct de ProductRule

                return RedirectToAction("Product"); // Redirige a la acción Product
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors); // Obtiene los errores del modelo
                foreach (var error in errors) // Imprime los mensajes de error en la consola
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var rule1 = new ProductRule(_configuration); // Crea una instancia de ProductRule pasando la configuración
            viewModel.Products = rule1.GetProducts() ?? new List<Product>(); // Obtiene los productos y los asigna al modelo de vista
            return View(viewModel); // Si el modelo no es válido, retorna a la vista con el modelo de vista actualizado
        }
    
    

        // Acción para mostrar el formulario de actualización del estado de órdenes
        [HttpGet]
        public IActionResult UpdateOrderStatus()
        {
            ViewBag.Title = "Órdenes"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Órdenes"; // Establece la página actual en ViewBag

            var rule = new OrderRule(_configuration); // Crea una instancia de OrderRule pasando la configuración
            var orders = rule.GetOrderDetails();  // Obtiene los detalles de todas las órdenes

            var viewModel = new OrderViewModel
            {
                UpdateOrder = new OrderDetailsViewModel(), // Inicializa la propiedad UpdateOrder como un nuevo objeto de OrderDetailsViewModel
                Orders = orders.ToList() // Convierte los detalles de órdenes en una lista y los asigna al modelo de vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción UpdateOrderStatus con el modelo viewModel
        }

        // Acción para procesar el formulario de actualización del estado de órdenes
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(OrderViewModel viewModel)
        {
            ViewBag.Title = "Órdenes"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Órdenes"; // Establece la página actual en ViewBag

            var rule = new OrderRule(_configuration); // Crea una instancia de OrderRule pasando la configuración
            rule.UpdateOrderStatus(viewModel.UpdateOrder);  // Actualiza el estado de la orden llamando al método UpdateOrderStatus de OrderRule
            TempData["SuccessMessage"] = "Orden actualizada correctamente!"; // Establece un mensaje de éxito en TempData
            return RedirectToAction("UpdateOrderStatus"); // Redirige a la acción UpdateOrderStatus para refrescar la vista
        }

        // Acción para mostrar la página de usuarios
        public IActionResult User()
        {
            ViewBag.Title = "Usuarios"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Usuarios"; // Establece la página actual en ViewBag
            var rule = new UserRule(_configuration); // Crea una instancia de UserRule pasando la configuración
            var users = rule.GetUsers(); // Obtiene los usuarios llamando al método GetUsers de UserRule

            var viewModel = new UserViewModel
            {
                Users = users, // Asigna la lista de usuarios al modelo de vista
                NewUser = new User() // Inicializa un nuevo objeto User para ser usado en la vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción User con el modelo viewModel
        }

        // Acción para mostrar la confirmación de eliminación de un usuario
        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            ViewBag.Title = "EliminarUsuario"; // Establece el título de la página en ViewBag
            ViewBag.Page = "EliminarUsuario"; // Establece la página actual en ViewBag
            var rule = new UserRule(_configuration); // Crea una instancia de UserRule pasando la configuración
            var user = rule.GetUserById(id); // Obtiene el usuario por ID llamando al método GetUserById de UserRule
            if (user == null) // Verifica si el usuario no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }
            var viewModel = new UserViewModel
            {
                NewUser = user, // Asigna el usuario obtenido al modelo de vista
                Users = rule.GetUsers() // Obtiene todos los usuarios y los asigna al modelo de vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción DeleteUser con el modelo viewModel
        }

        // Acción para procesar la eliminación de un usuario
        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteConfirmedUser(int id)
        {
            var rule = new UserRule(_configuration); // Crea una instancia de UserRule pasando la configuración
            rule.DeleteUser(id); // Elimina el usuario llamando al método DeleteUser de UserRule
            return RedirectToAction("User"); // Redirige a la acción User
        }

        // Acción para mostrar la página de consultas
        public IActionResult Contact()
        {
            ViewBag.Title = "Consultas"; // Establece el título de la página en ViewBag
            ViewBag.Page = "Consultas"; // Establece la página actual en ViewBag

            var rule = new ContactRule(_configuration); // Crea una instancia de ContactRule pasando la configuración
            var viewModel = new ContactViewModel
            {
                Contacts = rule.GetQuerys() // Obtiene las consultas y las asigna al modelo de vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción Contact con el modelo viewModel
        }

        // Acción para mostrar la confirmación de eliminación de una consulta
        [HttpGet]
        public IActionResult DeleteQuery(int id)
        {
            ViewBag.Title = "EliminarConsulta"; // Establece el título de la página en ViewBag
            ViewBag.Page = "EliminarConsulta"; // Establece la página actual en ViewBag
            var rule = new ContactRule(_configuration); // Crea una instancia de ContactRule pasando la configuración
            var query = rule.GetQueryById(id); // Obtiene la consulta por ID llamando al método GetQueryById de ContactRule
            if (query == null) // Verifica si la consulta no existe
            {
                return NotFound(); // Retorna un resultado 404 Not Found
            }
            var viewModel = new ContactViewModel
            {
                NewContact = query, // Asigna la consulta obtenida al modelo de vista
                Contacts = rule.GetQuerys() // Obtiene todas las consultas y las asigna al modelo de vista
            };

            return View(viewModel); // Devuelve la vista correspondiente a la acción DeleteQuery con el modelo viewModel
        }

        // Acción para procesar la eliminación de una consulta
        [HttpPost, ActionName("DeleteQuery")]
        public IActionResult DeleteConfirmedQuery(int id)
        {
            var rule = new ContactRule(_configuration); // Crea una instancia de ContactRule pasando la configuración
            rule.DeleteQuery(id); // Elimina la consulta llamando al método DeleteQuery de ContactRule
            return RedirectToAction("Contact"); // Redirige a la acción Contact
        }

        // Acción para mostrar el reporte de ventas con paginación
        public IActionResult SellingReport(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 8)
        {
            ViewBag.Title = "ReporteVenta"; // Establece el título de la página en ViewBag
            ViewBag.Page = "ReporteVenta"; // Establece la página actual en ViewBag

            var reportRule = new SellingReportRule(_configuration); // Crea una instancia de SellingReportRule pasando la configuración
            List<SellingReport> reportData;

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value) // Verifica si las fechas de inicio y fin son válidas
            {
                reportData = reportRule.GetReport(fromDate.Value, toDate.Value); // Obtiene el reporte de ventas entre las fechas dadas
                ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd"); // Establece la fecha de inicio en ViewBag
                ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd"); // Establece la fecha de fin en ViewBag
            }
            else
            {
                reportData = reportRule.GetAllReports(); // Obtiene todos los reportes si las fechas no son válidas
                ViewBag.FromDate = null; // Establece la fecha de inicio en ViewBag como null
                ViewBag.ToDate = null; // Establece la fecha de fin en ViewBag como null
            }

            decimal totalVendido = reportData.Sum(r => r.TotalPrice); // Calcula el total vendido sumando el precio total de todos los reportes
            ViewBag.TotalVendido = totalVendido; // Establece el total vendido en ViewBag

            // Paginación
            var totalItems = reportData.Count; // Cuenta el número total de items en el reporte
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // Calcula el número total de páginas
            var itemsOnPage = reportData.Skip((page - 1) * pageSize).Take(pageSize).ToList(); // Obtiene los items para la página actual

            ViewBag.CurrentPage = page; // Establece la página actual en ViewBag
            ViewBag.TotalPages = totalPages; // Establece el total de páginas en ViewBag

            return View(itemsOnPage); // Devuelve la vista correspondiente al reporte de ventas con los items de la página actual
        }






    }

	



}