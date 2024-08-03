using Dapper; // Importa Dapper, una biblioteca ORM para realizar consultas a bases de datos de manera sencilla
using FastFood.Models.DashboardAdmin; // Importa el espacio de nombres que contiene el modelo DashboardViewModel
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server
using Microsoft.Extensions.Configuration; // Importa el espacio de nombres para manejar la configuración
using System.Collections.Generic; // Importa el espacio de nombres para trabajar con colecciones genéricas
using System.Linq; // Importa el espacio de nombres para operaciones LINQ

namespace FastFood.Rules
{
    // Define la clase DashboardRule que maneja las reglas de negocio relacionadas con el panel de administración
    public class DashboardRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public DashboardRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para obtener los datos del panel de administración
        public DashboardViewModel GetDashboardData()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
                using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
                {
                    connection.Open(); // Abre la conexión a la base de datos

                    // Ejecuta una consulta para obtener el número total de categorías
                    var totalCategories = connection.QuerySingle<int>("SELECT COUNT(*) FROM Categories");
                    // Ejecuta una consulta para obtener el número total de productos
                    var totalProducts = connection.QuerySingle<int>("SELECT COUNT(*) FROM Products");
                    // Ejecuta una consulta para obtener el número total de órdenes
                    var totalOrders = connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders");
                    // Ejecuta una consulta para obtener el número de órdenes entregadas
                    var deliveredOrders = connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders WHERE Status = 'Entregado'");
                    // Ejecuta una consulta para obtener el número de órdenes pendientes
                    var pendingOrders = connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders WHERE Status = 'Pendiente'");
                    // Ejecuta una consulta para obtener el número total de usuarios
                    var totalUsers = connection.QuerySingle<int>("SELECT COUNT(*) FROM Users");
                    // Ejecuta una consulta para obtener la suma total de las ventas de órdenes entregadas
                    var totalSold = connection.QuerySingle<decimal>("SELECT ISNULL(SUM(Amount), 0) FROM Orders WHERE Status = 'Entregado'");
                    // Ejecuta una consulta para obtener el número total de comentarios o consultas de contacto
                    var totalFeedbacks = connection.QuerySingle<int>("SELECT COUNT(*) FROM Contact");

                    // Crea y devuelve un nuevo objeto DashboardViewModel con los datos obtenidos
                    return new DashboardViewModel
                    {
                        TotalCategories = totalCategories, // Asigna el total de categorías
                        TotalProducts = totalProducts, // Asigna el total de productos
                        TotalOrders = totalOrders, // Asigna el total de órdenes
                        DeliveredOrders = deliveredOrders, // Asigna el número de órdenes entregadas
                        PendingOrders = pendingOrders, // Asigna el número de órdenes pendientes
                        TotalUsers = totalUsers, // Asigna el total de usuarios
                        TotalSold = totalSold, // Asigna el total vendido
                        TotalFeedbacks = totalFeedbacks // Asigna el total de comentarios o consultas
                    };
                }
            }
            catch (Exception ex)
            {
                // Registro del error o manejo
                Console.WriteLine($"Error en GetDashboardData: {ex.Message}"); // Imprime el mensaje de error en la consola
                throw; // Lanza la excepción para que pueda ser manejada por el llamador
            }
        }
    }
}