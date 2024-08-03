using FastFood.Models; // Importa el espacio de nombres que contiene los modelos de datos, como User, Contact, etc.
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para la funcionalidad del controlador en ASP.NET Core
using System.Diagnostics; // Importa el espacio de nombres para el seguimiento de actividades del sistema
using FastFood.Rules; // Importa el espacio de nombres que contiene las reglas de negocio, como ProductRule, UserRule, etc.
using Microsoft.AspNetCore.Authentication.Cookies; // Importa el espacio de nombres para la autenticaci�n basada en cookies
using Microsoft.AspNetCore.Authentication; // Importa el espacio de nombres para la autenticaci�n en ASP.NET Core
using Microsoft.AspNetCore.Mvc.ModelBinding; // Importa el espacio de nombres para la manipulaci�n del ModelState
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
        private readonly IConfiguration _configuration; // Configuraci�n de la aplicaci�n
        private readonly IWebHostEnvironment _webHostEnvironment; // Informaci�n del entorno del host
        private readonly ISecurityServices _seguridad; // Servicios de seguridad para autenticaci�n y autorizaci�n

        // Constructor que inicializa los servicios y configuraciones
        public UserController(ISecurityServices seguridad, ILogger<UserController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _seguridad = seguridad; // Asigna el servicio de seguridad
            _logger = logger; // Asigna el registrador de eventos
            _configuration = configuration; // Asigna la configuraci�n de la aplicaci�n
            _webHostEnvironment = webHostEnvironment; // Asigna el entorno del host
        }

        // Acci�n para mostrar la p�gina de inicio
        public IActionResult Index()
        {
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule con la configuraci�n
            var products = rule.GetActiveProducts(); // Obtiene los productos activos
            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule con la configuraci�n
            var categories = rule2.GetActiveCategories(); // Obtiene las categor�as activas

            // Asocia cada producto con su nombre de categor�a
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Encuentra la categor�a correspondiente al producto
                if (category != null)
                {
                    product.CategoryName = category.Name; // Asigna el nombre de la categor�a al producto
                }
            }

            // Crea el modelo de vista para la p�gina de inicio
            var viewModel = new MenuViewModel
            {
                Categories = categories, // Asigna las categor�as al modelo de vista
                Products = products // Asigna los productos al modelo de vista
            };

            return View(viewModel); // Devuelve la vista Index con el modelo de vista
        }

        // Acci�n para mostrar el men�
        public IActionResult Menu()
        {
            var rule = new ProductRule(_configuration); // Crea una instancia de ProductRule con la configuraci�n
            var products = rule.GetActiveProducts(); // Obtiene los productos activos
            var rule2 = new CategoryRule(_configuration); // Crea una instancia de CategoryRule con la configuraci�n
            var categories = rule2.GetActiveCategories(); // Obtiene las categor�as activas

            // Asocia cada producto con su nombre de categor�a
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Encuentra la categor�a correspondiente al producto
                if (category != null)
                {
                    product.CategoryName = category.Name; // Asigna el nombre de la categor�a al producto
                }
            }

            // Crea el modelo de vista para la p�gina del men�
            var viewModel = new MenuViewModel
            {
                Categories = categories, // Asigna las categor�as al modelo de vista
                Products = products // Asigna los productos al modelo de vista
            };

            return View(viewModel); // Devuelve la vista Menu con el modelo de vista
        }

        // Acci�n para mostrar la p�gina de informaci�n sobre la empresa
        public IActionResult About()
        {
            return View(); // Devuelve la vista About
        }

        // Acci�n para mostrar la p�gina de contacto
        public IActionResult Contact()
        {
            return View(); // Devuelve la vista Contact
        }

        // Acci�n para manejar la solicitud POST del formulario de contacto
        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (ModelState.IsValid) // Verifica si el modelo de contacto es v�lido
            {
                var rule = new ContactRule(_configuration); // Crea una instancia de ContactRule con la configuraci�n
                rule.AddQuery(contact); // Agrega la consulta de contacto a la base de datos
                TempData["SuccessMessage"] = "Consulta enviada con �xito!"; // Establece un mensaje de �xito en TempData
                return RedirectToAction("Contact"); // Redirige a la acci�n Contact
            }
            return View(contact); // Devuelve la vista Contact con el modelo actualizado
        }

        // Acci�n para mostrar la p�gina de privacidad
        public IActionResult Privacy()
        {
            return View(); // Devuelve la vista Privacy
        }

        // Acci�n para manejar los errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Devuelve la vista Error con el modelo ErrorViewModel
        }

        // Acci�n para mostrar la p�gina de inicio de sesi�n
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Devuelve la vista Login
        }

        // Acci�n para manejar la solicitud POST del formulario de inicio de sesi�n
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public IActionResult Login(string email, string password)
        {
            try
            {
                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuraci�n
                User userConectado = _seguridad.Login(email, password); // Intenta autenticar al usuario

                // Verifica si el usuario existe
                if (userConectado == null)
                {
                    ModelState.AddModelError("Email", "El correo electr�nico no existe."); // Agrega un error al modelo si el correo electr�nico no existe
                    return View(); // Devuelve la vista Login
                }

                // Verifica si la contrase�a es correcta
                if (!rule.ValidarPassword(userConectado, password))
                {
                    ModelState.AddModelError("Password", "Contrase�a incorrecta."); // Agrega un error al modelo si la contrase�a es incorrecta
                    return View(); // Devuelve la vista Login
                }

                // Crea Claims para la autenticaci�n
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, userConectado.Name), // Agrega el nombre del usuario
                    new Claim(ClaimTypes.StreetAddress, userConectado.Address), // Agrega la direcci�n del usuario
                    new Claim(ClaimTypes.NameIdentifier, userConectado.UserName) // Agrega el nombre de usuario
                };

                // Crea ClaimsIdentity y ClaimsPrincipal para el usuario
                ClaimsIdentity identidad = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal userPrincipal = new(identidad);

                // Establece las propiedades de autenticaci�n
                AuthenticationProperties properties = new()
                {
                    IsPersistent = false // No persiste la autenticaci�n entre sesiones
                };
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, properties); // Establece la autenticaci�n en la solicitud HTTP

                // Registra la conexi�n del usuario
                _logger.LogCritical("El usuario {user} se conect� correctamente", userConectado.UserName);

                // Establece la sesi�n del usuario
                this.SetSession("USUARIO", userConectado);

                return RedirectToAction("Index"); // Redirige a la acci�n Index
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error inesperado al intentar iniciar sesi�n."); // Registra un error inesperado

                // Agrega un mensaje de error general al ModelState
                ModelState.AddModelError("", "Ha ocurrido un error inesperado. Por favor, int�ntelo de nuevo m�s tarde.");
                return View(); // Devuelve la vista Login
            }
        }

        // Acci�n para cerrar sesi�n
        [HttpGet]
        public IActionResult Logout()
        {
            // Elimina el usuario de la sesi�n
            this.SetSession<User>("USUARIO", null);

            // Cierra la sesi�n del usuario
            HttpContext.SignOutAsync();

            return RedirectToAction("Index"); // Redirige a la acci�n Index
        }

        // Acci�n para reiniciar la sesi�n
        [HttpGet]
        public IActionResult Restart()
        {
            // Elimina todas las variables de la sesi�n
            this.HttpContext.Session.Clear();

            // Cierra la sesi�n del usuario
            HttpContext.SignOutAsync();

            return RedirectToAction("Inicio", "Login"); // Redirige a la acci�n Inicio en el controlador Login
        }

        // Acci�n para mostrar la p�gina de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View(new User()); // Devuelve la vista Register con un nuevo objeto User
        }

        // Acci�n para manejar la solicitud POST del formulario de registro
        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid) // Verifica si el modelo de usuario es v�lido
            {
                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuraci�n

                // Verifica si el correo electr�nico ya est� registrado
                if (rule.GetUsuarioFromEmail(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Este correo electr�nico ya est� registrado."); // Agrega un error al modelo si el correo electr�nico ya est� registrado
                }
                else
                {
                    // Maneja la carga del archivo de imagen
                    if (model.ImageFile != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName); // Obtiene el nombre del archivo sin extensi�n
                        var extension = Path.GetExtension(model.ImageFile.FileName); // Obtiene la extensi�n del archivo
                        var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}"; // Genera un nuevo nombre de archivo �nico
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

                    model.CreatedDate = DateTime.Now; // Asigna la fecha de creaci�n

                    model.Password = rule.HashPassword(model.Password); // Hash de la contrase�a
                    rule.CreateUser(model); // Crea el usuario en la base de datos

                    TempData["SuccessMessage"] = "Usuario creado Correctamente"; // Establece un mensaje de �xito en TempData

                    return RedirectToAction("Login"); // Redirige a la acci�n Login
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

        // Acci�n para mostrar el perfil del usuario
        [HttpGet]
        public IActionResult UserProfile()
        {
            if (HttpContext.Session.IsAvailable) // Verifica si la sesi�n est� disponible
            {
                var user = HttpContext.Session.Get<User>("USUARIO"); // Obtiene el usuario de la sesi�n

                if (user != null)
                {
                    var rule = new OrderRule(_configuration); // Crea una instancia de OrderRule con la configuraci�n
                    var orders = rule.GetOrderDetailsById(user.UserId); // Obtiene los detalles de las �rdenes del usuario

                    if (user.TipoUsuario == TipoUsuario.Usuario) // Verifica el tipo de usuario
                    {
                        var viewModel = new UserProfileViewModel
                        {
                            Orders = orders, // Asigna las �rdenes al modelo de vista
                            User = user // Asigna el usuario al modelo de vista
                        };

                        return View(viewModel); // Devuelve la vista UserProfile con el modelo de vista
                    }
                    else
                    {
                        return View("RedirectToAdmin"); // Redirige a la vista RedirectToAdmin si el usuario no es de tipo Usuario
                    }
                }

                return RedirectToAction("Login", "User"); // Redirige a la acci�n Login si el usuario no est� en la sesi�n
            }

            return RedirectToAction("Login", "User"); // Redirige a la acci�n Login si la sesi�n no est� disponible
        }

        // Acci�n para mostrar el formulario de edici�n del perfil
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuraci�n
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

        // Acci�n para manejar la solicitud POST del formulario de edici�n del perfil
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid) // Verifica si el modelo de edici�n es v�lido
            {
                // Maneja la carga del archivo de imagen
                if (model.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName); // Obtiene el nombre del archivo sin extensi�n
                    var extension = Path.GetExtension(model.ImageFile.FileName); // Obtiene la extensi�n del archivo
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}"; // Genera un nuevo nombre de archivo �nico
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

                var rule = new UserRule(_configuration); // Crea una instancia de UserRule con la configuraci�n
                var user = rule.GetUsuarioFromEmail(model.Email); // Obtiene el usuario por correo electr�nico

                // Actualiza las propiedades del usuario
                user.Name = model.Name;
                user.UserName = model.UserName;
                user.Mobile = model.Mobile;
                user.Email = model.Email;
                user.Address = model.Address;
                user.ImageUrl = model.ImageUrl ?? user.ImageUrl; // Mantiene la URL de la imagen si no se proporciona una nueva
                user.PostCode = model.PostCode;

                rule.UpdateUser(user); // Actualiza el usuario en la base de datos

                // Crea una lista de Claims para la autenticaci�n
                List<Claim> claims = new List<Claim>
                {
                    new (ClaimTypes.Name, user.Name), // Agrega el nombre del usuario
                    new (ClaimTypes.StreetAddress, user.Address), // Agrega la direcci�n del usuario
                    new (ClaimTypes.NameIdentifier, user.UserName) // Agrega el nombre de usuario
                };

                // Crea ClaimsIdentity y ClaimsPrincipal para el usuario
                ClaimsIdentity identidad = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal userPrincipal = new(identidad);

                this.SetSession("USUARIO", user); // Establece la sesi�n del usuario
                TempData["SuccessMessage"] = "Usuario actualizado correctamente"; // Establece un mensaje de �xito en TempData

                return RedirectToAction("UserProfile"); // Redirige a la acci�n UserProfile
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