using FastFood.Models;
using FastFood.Rules; // Importa el espacio de nombres que contiene las reglas de negocio, como PaymentRule, OrderRule, etc.
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para la funcionalidad del controlador en ASP.NET Core
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para manejar excepciones de SQL Server
using System.Data; // Importa el espacio de nombres para clases relacionadas con datos
using System.Data.SqlTypes; // Importa el espacio de nombres para tipos de datos SQ
using web.Utiles; // Importa el espacio de nombres que contiene utilidades para manejar sesiones, como Get y Set

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration; // Configuración de la aplicación, inyectada a través del constructor
    private readonly IWebHostEnvironment _webHostEnvironment; // Información del entorno del host, inyectada a través del constructor

    private const string CartSessionKey = "Cart"; // Clave de sesión utilizada para almacenar el carrito de compras

    // Constructor que inicializa la configuración y el entorno del host
    public PaymentController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _configuration = configuration; // Asigna la configuración al campo privado
        _webHostEnvironment = webHostEnvironment; // Asigna el entorno del host al campo privado
    }

    // Acción para mostrar la página de pago
    [HttpGet] // Indica que esta acción maneja solicitudes HTTP GET
    public IActionResult Payment()
    {
        var cartItems = GetCart(); // Obtiene los artículos del carrito de la sesión
        var total = cartItems.Sum(item => item.Price * item.Quantity); // Calcula el total del carrito sumando el precio de cada artículo por su cantidad

        // Crea un modelo de vista para la página de pago
        var viewModel = new PaymentViewModel
        {
            CartItems = cartItems, // Asigna los artículos del carrito al modelo de vista
            Total = total, // Asigna el total calculado al modelo de vista
            Payment = new Payment() // Inicializa un nuevo objeto Payment en el modelo de vista
        };

        return View(viewModel); // Devuelve la vista Payment con el modelo de vista
    }

    // Acción para confirmar el pago
    [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
    public IActionResult ConfirmPayment(PaymentViewModel model)
    {
        model.CartItems = GetCart(); // Obtiene los artículos del carrito de la sesión
        model.Total = model.CartItems.Sum(item => item.Price * item.Quantity); // Calcula el total del carrito

        // Verifica si la información del pago es proporcionada
        if (model.Payment == null)
        {
            ModelState.AddModelError(string.Empty, "Payment information is required."); // Agrega un error al modelo si la información del pago es nula
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        // Verifica si hay artículos en el carrito
        if (model.CartItems == null || !model.CartItems.Any())
        {
            ModelState.AddModelError(string.Empty, "Cart items are required."); // Agrega un error al modelo si el carrito está vacío
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        // Valida y asigna la fecha de expiración del pago
        var expireMonth = model.Payment.ExpireMonth;
        var expireYear = model.Payment.ExpireYear;
        var expireDate = new DateTime(2000 + expireYear, expireMonth, 1); // Crea una fecha de expiración con el año y mes proporcionados

        var paymentRule = new PaymentRule(_configuration); // Crea una instancia de PaymentRule con la configuración

        // Asigna valores a la información del pago
        model.Payment.Amount = model.Total;
        model.Payment.PaymentMode = "Card";
        model.Payment.ExpireDate = expireDate;

        try
        {
            paymentRule.InsertPayment(model.Payment); // Intenta insertar el pago en la base de datos

            var user = HttpContext.Session.Get<User>("USUARIO"); // Obtiene la información del usuario de la sesión
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User information is required."); // Agrega un error al modelo si la información del usuario es nula
                return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
            }

            var orderRule = new OrderRule(_configuration); // Crea una instancia de OrderRule con la configuración

            // Crea una nueva orden con los detalles proporcionados
            var order = new Order
            {
                OrderNo = Guid.NewGuid().ToString(), // Genera un número de orden único
                UserId = user.UserId, // Asigna el ID del usuario a la orden
                Status = "Pendiente", // Asigna el estado de la orden
                PaymentId = paymentRule.GetLatestPaymentId(), // Obtiene el último ID de pago
                OrderDate = DateTime.Now, // Asigna la fecha actual como la fecha de la orden
                Amount = model.Total, // Asigna el total de la orden
                OrderDetails = model.CartItems.Select(cartItem => new OrderDetail // Crea una lista de detalles de la orden para cada artículo del carrito
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                }).ToList()
            };

            try
            {
                orderRule.InsertOrder(order); // Intenta insertar la orden y sus detalles en la base de datos
            }
            catch (SqlTypeException)
            {
                ModelState.AddModelError(string.Empty, "Error en la fecha de la orden."); // Agrega un error al modelo si ocurre una excepción de tipo SQL
                return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
            }
        }
        catch (SqlException)
        {
            ModelState.AddModelError(string.Empty, "Error en el pago."); // Agrega un error al modelo si ocurre una excepción de SQL
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        // Limpiar el carrito después de confirmar el pago
        HttpContext.Session.Remove("Cart");

        // Redirigir a la vista de perfil del usuario
        return RedirectToAction("UserProfile", "User");
    }

    // Acción para confirmar una orden con pago en efectivo
    [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
    public IActionResult ConfirmOrder(PaymentViewModel model)
    {
        model.CartItems = GetCart(); // Obtiene los artículos del carrito de la sesión
        model.Total = model.CartItems.Sum(item => item.Price * item.Quantity); // Calcula el total del carrito

        // Verifica si la información del pago es proporcionada
        if (model.Payment == null)
        {
            ModelState.AddModelError(string.Empty, "Payment information is required."); // Agrega un error al modelo si la información del pago es nula
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        // Verifica si hay artículos en el carrito
        if (model.CartItems == null || !model.CartItems.Any())
        {
            ModelState.AddModelError(string.Empty, "Cart items are required."); // Agrega un error al modelo si el carrito está vacío
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        var simpleRule = new SimplePaymentRule(_configuration); // Crea una instancia de SimplePaymentRule con la configuración

        // Asigna valores a la información del pago para pago en efectivo
        model.Payment.Amount = model.Total;
        model.Payment.Cvv = 0; // El campo CVV no se usa para pagos en efectivo
        model.Payment.CardNo = ""; // El número de tarjeta no se usa para pagos en efectivo
        model.Payment.PaymentMode = "Cash";
        model.Payment.ExpireDate = DateTime.Now; // La fecha de expiración es la fecha actual ya que es un pago al delivery

        try
        {
            simpleRule.InsertSimplePayment(model.Payment); // Intenta insertar el pago en efectivo en la base de datos
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Error al guardar la información del pago."); // Agrega un error al modelo si ocurre una excepción
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        var user = HttpContext.Session.Get<User>("USUARIO"); // Obtiene la información del usuario de la sesión
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User information is required."); // Agrega un error al modelo si la información del usuario es nula
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        var orderRule = new OrderRule(_configuration); // Crea una instancia de OrderRule con la configuración

        // Crea una nueva orden con los detalles proporcionados
        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString(), // Genera un número de orden único
            UserId = user.UserId, // Asigna el ID del usuario a la orden
            Status = "Pendiente", // Asigna el estado de la orden
            PaymentId = simpleRule.GetLatestPaymentId(), // Obtiene el último ID de pago para pago en efectivo
            OrderDate = DateTime.Now, // Asigna la fecha actual como la fecha de la orden
            Amount = model.Total, // Asigna el total de la orden
            OrderDetails = model.CartItems.Select(cartItem => new OrderDetail // Crea una lista de detalles de la orden para cada artículo del carrito
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
            }).ToList()
        };

        try
        {
            orderRule.InsertOrder(order); // Intenta insertar la orden y sus detalles en la base de datos
        }
        catch (SqlTypeException)
        {
            ModelState.AddModelError(string.Empty, "Error en la fecha de la orden."); // Agrega un error al modelo si ocurre una excepción de tipo SQL
            return View("Payment", model); // Devuelve la vista Payment con el modelo actualizado
        }

        // Limpiar el carrito después de confirmar el pago
        HttpContext.Session.Remove("Cart");

        // Redirigir a la vista de perfil del usuario
        return RedirectToAction("UserProfile", "User");
    }

    // Método privado para obtener el último ID de pago
    private int GetLatestPaymentId()
    {
        var rule = new PaymentRule(_configuration); // Crea una instancia de PaymentRule con la configuración
        return rule.GetLatestPaymentId(); // Llama al método para obtener el último ID de pago
    }

    // Método privado para obtener el carrito de la sesión
    private List<CartItem> GetCart()
    {
        var cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey); // Obtiene el carrito de la sesión utilizando la clave de sesión
        if (cart == null) // Si el carrito no existe en la sesión
        {
            cart = new List<CartItem>(); // Inicializa un nuevo carrito vacío
        }
        return cart; // Retorna el carrito
    }
}