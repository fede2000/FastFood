
using FastFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using web.Utiles;

namespace web.Componentes;

public class ButtonUserComponent : ViewComponent
{
  public IViewComponentResult Invoke()
  {
    if (HttpContext.Session.IsAvailable)
    {
            //obtenemos el user session
      var user = HttpContext.Session.Get<User>("USUARIO");

            if (user != null)
            {
                //Existe un user logueado, verificamos si es del tipo usuario
                if (user.TipoUsuario == TipoUsuario.Usuario)
                    // el user es de tipo usuario, retornamos el boton de logout para que el user salga de la sesion, /Views/Shared/Components/ButtonUserComponent/Logout.cshtml
                    return View("Logout", user);

            }
            // si no existe un user en la sesion retornamos el boton de login,/Views/Shared/Components/ButtonUserComponent/Login.cshtml
            return View("Login");
    }
        //  si el user no esta logueado retornar un fragmento generico,/Views/Shared/Components/ButtonUserComponent/Login.cshtml
        return View("Login");
  }
}
