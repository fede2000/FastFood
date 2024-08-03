
using FastFood.Models;
using FastFood.Models.User;
using Microsoft.AspNetCore.Mvc;
using web.Utiles;

namespace web.Componentes;

public class UserInfoComponent : ViewComponent
{
  public IViewComponentResult Invoke()
  {
        //validamos si hay una sesion disponible
    if (HttpContext.Session.IsAvailable)
    {
            //obtenemos el user session
      var user = HttpContext.Session.Get<User>("USUARIO");
            //si es null retornamos usergenerico
            if (user != null)
            {
                //si el user es de tipo usuario retornamos la vista usuario
                if (user.TipoUsuario == TipoUsuario.Usuario)
                    return View("Usuario", user);
                else
                {
                    //si el user es de tipo admin, redirigimos a la vista del administrador
                    return View("RedirectToAdmin");
                }

            }

            return View("UserGenerico");
    }
    //  si el user no esta logueado retornar un fragmento generico
    return View("UserGenerico");
  }
}
