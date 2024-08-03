using Microsoft.AspNetCore.Mvc;
using web.Utiles;
using System.Collections.Generic;
using System.Linq;
using FastFood.Models.Cart;

namespace web.Componentes
{
    public class CartCountComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            //verificamos si hay una session disponible
            if (HttpContext.Session.IsAvailable)
            {
                //obtenemos la informacion del cart session
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
                if (cart != null && cart.Any())
                {
                    var count = cart.Sum(item => item.Quantity);
                    //retornamos a la vista parcial el numero de producto del carrito, /Views/Shared/Components/CartCountComponent/Default.cshtml
                    return View(count);
                }
            }
            // Retornar contenido vacío si no hay productos en el carrito
            return View(0);
        }
    }
}