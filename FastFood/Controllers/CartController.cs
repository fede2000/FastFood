using FastFood.Models.Cart;
using Microsoft.AspNetCore.Authorization; // Importa el espacio de nombres para la autorización en ASP.NET Core
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para la funcionalidad del controlador en ASP.NET Core
using web.Utiles; // Importa el espacio de nombres que contiene utilidades para manejar sesiones, como Get y Set

namespace FastFood.Controllers
{
    [Authorize] // Atributo que requiere que el usuario esté autenticado para acceder a las acciones de este controlador
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart"; // Clave de sesión utilizada para almacenar el carrito de compras en la sesión

        // Acción para mostrar el carrito de compras
        public IActionResult Cart()
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            return View(cart); // Devuelve la vista Cart con el carrito como modelo
        }

        // Acción para agregar un producto al carrito
        [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
        [ValidateAntiForgeryToken] // Añade protección contra ataques de falsificación de solicitudes entre sitios (CSRF)
        public IActionResult AddToCart([FromBody] CartItem cartItem)
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            var existingCartItem = cart.Find(item => item.ProductId == cartItem.ProductId); // Busca un producto existente en el carrito

            if (existingCartItem != null) // Si el producto ya está en el carrito
            {
                existingCartItem.Quantity++; // Incrementa la cantidad del producto
            }
            else // Si el producto no está en el carrito
            {
                cart.Add(new CartItem // Crea un nuevo CartItem y lo agrega al carrito
                {
                    ProductId = cartItem.ProductId,
                    Name = cartItem.Name,
                    Image = cartItem.Image,
                    Price = cartItem.Price,
                    Quantity = 1 // Inicializa la cantidad del producto en 1
                });
            }

            SaveCart(cart); // Guarda el carrito actualizado en la sesión
            return Json(new { success = true, message = "Producto agregado al carrito" }); // Retorna una respuesta JSON indicando éxito
        }

        // Acción para incrementar la cantidad de un producto en el carrito
        [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
        [ValidateAntiForgeryToken] // Añade protección contra ataques de falsificación de solicitudes entre sitios (CSRF)
        public IActionResult IncrementQuantity([FromBody] int productId)
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            var cartItem = cart.Find(item => item.ProductId == productId); // Busca el producto en el carrito

            if (cartItem != null) // Si el producto está en el carrito
            {
                cartItem.Quantity++; // Incrementa la cantidad del producto
            }

            SaveCart(cart); // Guarda el carrito actualizado en la sesión
            return Json(new { success = true, message = "Producto incrementado" }); // Retorna una respuesta JSON indicando éxito
        }

        // Acción para eliminar un producto del carrito
        [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            var cartItem = cart.Find(item => item.ProductId == productId); // Busca el producto en el carrito

            if (cartItem != null) // Si el producto está en el carrito
            {
                cart.Remove(cartItem); // Elimina el producto del carrito
                SaveCart(cart); // Guarda el carrito actualizado en la sesión
                return Json(new { success = true, message = "Producto eliminado del carrito." }); // Retorna una respuesta JSON indicando éxito
            }
            return Json(new { success = false, message = "Product not found in cart." }); // Retorna una respuesta JSON indicando que el producto no fue encontrado
        }

        // Acción para decrementar la cantidad de un producto en el carrito
        [HttpPost] // Indica que esta acción maneja solicitudes HTTP POST
        [ValidateAntiForgeryToken] // Añade protección contra ataques de falsificación de solicitudes entre sitios (CSRF)
        public IActionResult DecrementQuantity([FromBody] int productId)
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            var cartItem = cart.Find(item => item.ProductId == productId); // Busca el producto en el carrito

            if (cartItem != null && cartItem.Quantity > 1) // Si el producto está en el carrito y su cantidad es mayor a 1
            {
                cartItem.Quantity--; // Decrementa la cantidad del producto
            }

            SaveCart(cart); // Guarda el carrito actualizado en la sesión
            return Json(new { success = true, message = "Quantity decremented." }); // Retorna una respuesta JSON indicando éxito
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

        // Método público para obtener el número total de productos en el carrito
        public int GetCartCount()
        {
            var cart = GetCart(); // Obtiene el carrito de compras de la sesión
            return cart.Sum(item => item.Quantity); // Suma las cantidades de todos los productos en el carrito
        }

        // Método privado para guardar el carrito en la sesión
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart); // Guarda el carrito en la sesión utilizando la clave de sesión
        }
    }
}