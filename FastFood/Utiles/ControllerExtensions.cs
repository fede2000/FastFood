using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace web.Utiles;

public static class ControllerExtensions
{
  /// <summary>
  /// Evitamos serializar tipos valor con este metodo...
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="src"></param>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void Set<T>(this ISession src, string key, T value) //where T: class
  {
    src?.SetString(key, JsonConvert.SerializeObject(value));
  }

  /// <summary>
  /// Obtiene una instancia del tipo T a partir de una clave en la sesion
  /// Si el valor no existe en la sesion, retorna el default(T)
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="src"></param>
  /// <param name="key"></param>
  /// <returns></returns>
  public static T Get<T>(this ISession src, string key) //where T: class
  {
    string valor = src?.GetString(key);

    return valor is null ? default : JsonConvert.DeserializeObject<T>(valor);
  }

  public static void Clear(this ISession src, string key)
  {
    src?.Remove(key);
  }

  public static T GetSession<T>(this ControllerBase src, string key)
  {
    return src.HttpContext.Session.Get<T>(key);
  }

  public static void SetSession<T>(this ControllerBase src, string key, T value)
  {
    src.HttpContext.Session.Set<T>(key, value);
  }
}
