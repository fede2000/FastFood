using FastFood.Models; // Importa el espacio de nombres que contiene los modelos de datos, como User, Contact, etc.
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para la funcionalidad del controlador en ASP.NET Core
using System.Diagnostics; // Importa el espacio de nombres para el seguimiento de actividades del sistema
using FastFood.Rules; // Importa el espacio de nombres que contiene las reglas de negocio, como ProductRule, UserRule, etc.
using Microsoft.AspNetCore.Authentication.Cookies; // Importa el espacio de nombres para la autenticación basada en cookies
using Microsoft.AspNetCore.Authentication; // Importa el espacio de nombres para la autenticación en ASP.NET Core
using Microsoft.AspNetCore.Mvc.ModelBinding; // Importa el espacio de nombres para la manipulación del ModelState
using System.Security.Claims; // Importa el espacio de nombres para trabajar con Claims
using Servicios; // Importa el espacio de nombres para servicios adicionales, como ISecurityServices
using web.Utiles; // Importa el espacio de nombres que contiene utilidades para manejar sesiones
using web.Models; // Importa el espacio de nombres que contiene modelos de vistas
using System.Data;
using FastFood.Models.Contact;
using FastFood.Models.User; // Importa el espacio de nombres para trabajar con datos

namespace FastFood.Controllers
{
    // Define el controlador UserController
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger; // Registra eventos del controlador
        private readonly IConfiguration _configuration; // Configuración de la aplicación
        private readonly IWebHostEnvironment _webHostEnvironment; // Información del entorno del host
        private readonly ISecurityServices _seguridad; // Servicios de seguridad para autenticación y autorización

        // Constructor que inicializa los servicios y configuraciones
        public UserController(ISecurityServices seguridad, ILogger<UserController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _seguridad = seguridad; // Asigna el servicio de seguridad
            _logger = logger; // Asigna el registrador de eventos
            _configuration = configuration; // Asigna la configuración de la aplicación
            _webHostEnvironment = webHostEnvironment; // Asigna el entorno del host
        }

        // Acción para mostrar la página de inicio
        public IActionResult Index()
        {
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule con la configuración
            var products = rule.GetActiveProducts(); // Obtiene los productos activos
            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule con la configuración
            var categories = rule2.GetActiveCategories(); // Obtiene las categorías activas

            // Asocia cada producto con su nombre de categoría
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Encuentra la categoría correspondiente al producto
                if (category != null)
                {
                    product.CategoryName = category.Name; // Asigna el nombre de la categoría al producto
                }
            }

            // Crea el modelo de vista para la página de inicio
            var viewModel = new MenuViewModel
            {
                Categories = categories, // Asigna las categorías al modelo de vista
                Products = products // Asigna los productos al modelo de vista
            };

            return View(viewModel); // Devuelve la vista Index con el modelo de vista
        }

        // Acción para mostrar el menú
        public IActionResult Menu()
        {
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule con la configuración
            var products = rule.GetActiveProducts(); // Obtiene los productos activos
            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule con la configuración
            var categories = rule2.GetActiveCategories(); // Obtiene las categorías activas

            // Asocia cada producto con su nombre de categoría
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Encuentra la categoría correspondiente al producto
                if (category != null)
                {
                    product.CategoryName = category.Name; // Asigna el nombre de la categoría al producto
                }
            }

            // Crea el modelo de vista para la página del menú
            var viewModel = new MenuViewModel
            {
                Categories = categories, // Asigna las categorías al modelo de vista
                Products = products // Asigna los productos al modelo de vista
            };

            return View(viewModel); // Devuelve la vista Menu con el modelo de vista
        }

        // Acción para mostrar la página de información sobre la empresa
        public IActionResult About()
        {
            return View(); // Devuelve la vista About
        }

        // Acción para mostrar la página de contacto
        public IActionResult Contact()
        {
            return View(); // Devuelve la vista Contact
        }

        // Acción para manejar la solicitud POST del formulario de contacto
        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (ModelState.IsValid) // Verifica si el modelo de contacto es válido
            {
                var rule = new ContactRule(_configuration); // Crea una instancia de ContactRule con la configuración
                rule.AddQuery(contact); // Agrega la consulta de contacto a la base de datos
                TempData["SuccessMessage"] = "Consulta enviada con éxito!"; // Establece un mensaje de éxito en TempData
                return RedirectToAction("Contact"); // Redirige a la acción Contact
            }
            return View(contact); // Devuelve la vista Contact con el modelo actualizado
        }

        // Acción para mostrar la página de privacidad
        public IActionResult Privacy()
        {
            return View(); // Devuelve la vista Privacy
        }

        // Acción para manejar los errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Devuelve la vista Error con el modelo ErrorViewModel
        }

        // Acción para mostrar la página de inicio de sesión
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Devuelve la vista Login
        }

        // Acción para manejar la solicitud POST del formulario de inicio de sesión
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public IActionResult Login(string email, string password)
        {
            try
            {
                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuración
                User userConectado = _seguridad.Login(email, password); // Intenta autenticar al usuario

                // Verifica si el usuario existe
                if (userConectado == null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico no existe."); // Agrega un error al modelo si el correo electrónico no existe
                    return View(); // Devuelve la vista Login
                }

                // Verifica si la contraseña es correcta
                if (!rule.ValidarPassword(userConectado, password))
                {
                    ModelState.AddModelError("Password", "Contraseña incorrecta."); // Agrega un error al modelo si la contraseña es incorrecta
                    return View(); // Devuelve la vista Login
                }

                // Crea Claims para la autenticación
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, userConectado.Name), // Agrega el nombre del usuario
                    new Claim(ClaimTypes.StreetAddress, userConectado.Address), // Agrega la dirección del usuario
                    new Claim(ClaimTypes.NameIdentifier, userConectado.UserName) // Agrega el nombre de usuario
                };

                // Crea ClaimsIdentity y ClaimsPrincipal para el usuario
                ClaimsIdentity identidad = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal userPrincipal = new(identidad);

                // Establece las propiedades de autenticación
                AuthenticationProperties properties = new()
                {
                    IsPersistent = false // No persiste la autenticación entre sesiones
                };
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, properties); // Establece la autenticación en la solicitud HTTP

                // Registra la conexión del usuario
                _logger.LogCritical("El usuario {user} se conectó correctamente", userConectado.UserName);

                // Establece la sesión del usuario
                this.SetSession("USUARIO", userConectado);

                return RedirectToAction("Index"); // Redirige a la acción Index
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error inesperado al intentar iniciar sesión."); // Registra un error inesperado

                // Agrega un mensaje de error general al ModelState
                ModelState.AddModelError("", "Ha ocurrido un error inesperado. Por favor, inténtelo de nuevo más tarde.");
                return View(); // Devuelve la vista Login
            }
        }

        // Acción para cerrar sesión
        [HttpGet]
        public IActionResult Logout()
        {
            // Elimina el usuario de la sesión
            this.SetSession<User>("USUARIO", null);

            // Cierra la sesión del usuario
            HttpContext.SignOutAsync();

            return RedirectToAction("Index"); // Redirige a la acción Index
        }

        // Acción para reiniciar la sesión
        [HttpGet]
        public IActionResult Restart()
        {
            // Elimina todas las variables de la sesión
            this.HttpContext.Session.Clear();

            // Cierra la sesión del usuario
            HttpContext.SignOutAsync();

            return RedirectToAction("Inicio", "Login"); // Redirige a la acción Inicio en el controlador Login
        }

        // Acción para mostrar la página de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View(new User()); // Devuelve la vista Register con un nuevo objeto User
        }

        // Acción para manejar la solicitud POST del formulario de registro
        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid) // Verifica si el modelo de usuario es válido
            {
                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuración

                // Verifica si el correo electrónico ya está registrado
                if (rule.GetUsuarioFromEmail(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado."); // Agrega un error al modelo si el correo electrónico ya está registrado
                }
                else
                {
                    // Maneja la carga del archivo de imagen
                    if (model.ImageFile != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName); // Obtiene el nombre del archivo sin extensión
                        var extension = Path.GetExtension(model.ImageFile.FileName); // Obtiene la extensión del archivo
                        var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}"; // Genera un nuevo nombre de archivo único
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads"); // Ruta de carga de archivos

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath); // Crea el directorio si no existe
                        }

                        var filePath = Path.Combine(uploadPath, newFileName); // Ruta completa del archivo

                        // Guarda el archivo en el servidor
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        // Actualiza la propiedad ImageUrl con la ruta del archivo
                        model.ImageUrl = $"/uploads/{newFileName}";
                    }

                    model.CreatedDate = DateTime.Now; // Asigna la fecha de creación

                    model.Password = rule.HashPassword(model.Password); // Hash de la contraseña
                    rule.CreateUser(model); // Crea el usuario en la base de datos

                    TempData["SuccessMessage"] = "Usuario creado Correctamente"; // Establece un mensaje de éxito en TempData

                    return RedirectToAction("Login"); // Redirige a la acción Login
                }
            }
            else
            {
                // Muestra los errores del modelo en la consola
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(model); // Devuelve la vista Register con el modelo actualizado
        }

        // Acción para mostrar el perfil del usuario
        [HttpGet]
        public IActionResult UserProfile()
        {
            if (HttpContext.Session.IsAvailable) // Verifica si la sesión está disponible
            {
                var user = HttpContext.Session.Get<User>("USUARIO"); // Obtiene el usuario de la sesión

                if (user != null)
                {
                    var rule = new OrderRule(_configuration); // Crea una instancia de OrderRule con la configuración
                    var orders = rule.GetOrderDetailsById(user.UserId); // Obtiene los detalles de las órdenes del usuario

                    if (user.TipoUsuario == TipoUsuario.Usuario) // Verifica el tipo de usuario
                    {
                        var viewModel = new UserProfileViewModel
                        {
                            Orders = orders, // Asigna las órdenes al modelo de vista
                            User = user // Asigna el usuario al modelo de vista
                        };

                        return View(viewModel); // Devuelve la vista UserProfile con el modelo de vista
                    }
                    else
                    {
                        return View("RedirectToAdmin"); // Redirige a la vista RedirectToAdmin si el usuario no es de tipo Usuario
                    }
                }

                return RedirectToAction("Login", "User"); // Redirige a la acción Login si el usuario no está en la sesión
            }

            return RedirectToAction("Login", "User"); // Redirige a la acción Login si la sesión no está disponible
        }

        // Acción para mostrar el formulario de edición del perfil
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuración
            var user = rule.GetUserById(id); // Obtiene el usuario por ID

            if (user == null)
            {
                return NotFound(); // Devuelve NotFound si el usuario no existe
            }

            // Crea el modelo de vista para editar el perfil
            var model = new EditProfileViewModel
            {
                Name = user.Name,
                UserName = user.UserName,
                Mobile = user.Mobile,
                Email = user.Email,
                Address = user.Address,
                PostCode = user.PostCode,
                ImageUrl = user.ImageUrl
            };

            return View(model); // Devuelve la vista EditProfile con el modelo de vista
        }

        // Acción para manejar la solicitud POST del formulario de edición del perfil
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid) // Verifica si el modelo de edición es válido
            {
                // Maneja la carga del archivo de imagen
                if (model.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName); // Obtiene el nombre del archivo sin extensión
                    var extension = Path.GetExtension(model.ImageFile.FileName); // Obtiene la extensión del archivo
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}"; // Genera un nuevo nombre de archivo único
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads"); // Ruta de carga de archivos

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath); // Crea el directorio si no existe
                    }

                    var filePath = Path.Combine(uploadPath, newFileName); // Ruta completa del archivo

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl con la ruta del archivo
                    model.ImageUrl = $"/uploads/{newFileName}";
                }

                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuración
                var user = rule.GetUsuarioFromEmail(model.Email); // Obtiene el usuario por correo electrónico

                // Actualiza las propiedades del usuario
                user.Name = model.Name;
                user.UserName = model.UserName;
                user.Mobile = model.Mobile;
                user.Email = model.Email;
                user.Address = model.Address;
                user.ImageUrl = model.ImageUrl ?? user.ImageUrl; // Mantiene la URL de la imagen si no se proporciona una nueva
                user.PostCode = model.PostCode;

                rule.UpdateUser(user); // Actualiza el usuario en la base de datos

                // Crea una lista de Claims para la autenticación
                List<Claim> claims = new List<Claim>
                {
                    new (ClaimTypes.Name, user.Name), // Agrega el nombre del usuario
                    new (ClaimTypes.StreetAddress, user.Address), // Agrega la dirección del usuario
                    new (ClaimTypes.NameIdentifier, user.UserName) // Agrega el nombre de usuario
                };

                // Crea ClaimsIdentity y ClaimsPrincipal para el usuario
                ClaimsIdentity identidad = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal userPrincipal = new(identidad);

                this.SetSession("USUARIO", user); // Establece la sesión del usuario
                TempData["SuccessMessage"] = "Usuario actualizado correctamente"; // Establece un mensaje de éxito en TempData

                return RedirectToAction("UserProfile"); // Redirige a la acción UserProfile
            }
            else
            {
                // Muestra los errores del modelo en la consola
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(model); // Devuelve la vista EditProfile con el modelo actualizado
        }
    }
}