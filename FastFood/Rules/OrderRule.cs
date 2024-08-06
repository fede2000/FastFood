using Dapper; // Importa Dapper, una biblioteca ORM para facilitar el acceso a la base de datos
using FastFood.Models;
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server
using Microsoft.Extensions.Configuration; // Importa el espacio de nombres para manejar la configuración
using System.Data; // Importa el espacio de nombres para trabajar con tipos de datos relacionados con la base de datos

namespace FastFood.Rules
{
    // Define la clase OrderRule que contiene las reglas y métodos para manejar las órdenes
    public class OrderRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public OrderRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para insertar una nueva orden en la base de datos
        public void InsertOrder(Order order)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para insertar la orden principal en la tabla Orders y recuperar el OrderId generado
                var queryInsertOrder = "INSERT INTO Orders (OrderNo, UserId, Status, PaymentId, OrderDate, Amount) VALUES (@OrderNo, @UserId, @Status, @PaymentId, @OrderDate, @Amount); SELECT CAST(SCOPE_IDENTITY() as int)";
                // Ejecuta la consulta e inserta la orden, luego asigna el OrderId generado a la propiedad OrderId del objeto
                order.OrderId = connection.QuerySingle<int>(queryInsertOrder, new
                {
                    order.OrderNo,
                    order.UserId,
                    order.Status,
                    order.PaymentId,
                    order.OrderDate,
                    order.Amount
                });

                // Itera sobre los detalles de la orden y los inserta en la tabla OrderDetails
                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderId = order.OrderId; // Asigna el OrderId generado a cada detalle
                    var queryInsertDetail = "INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price) VALUES (@OrderId, @ProductId, @Quantity, @Price)";
                    // Ejecuta la consulta para insertar cada detalle de la orden
                    connection.Execute(queryInsertDetail, new
                    {
                        detail.OrderId,
                        detail.ProductId,
                        detail.Quantity,
                        detail.Price
                    });
                }
            }
        }

        // Método para actualizar el estado de una orden en la base de datos
        public void UpdateOrderStatus(OrderDetailsViewModel updateOrder)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                // Consulta para actualizar el estado de la orden
                var query = @"
                    UPDATE Orders
                    SET Status = @Status
                    WHERE OrderId = @OrderId";
                // Ejecuta la consulta para actualizar el estado de la orden especificada
                connection.Execute(query, new { Status = updateOrder.Status, OrderId = updateOrder.OrderId });
            }
        }

        // Método para obtener todas las órdenes de un usuario específico
        public IEnumerable<Order> GetOrderById(int userId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                // Consulta para seleccionar todas las órdenes del usuario ordenadas por la fecha de creación
                var querySelect = "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY CreatedDate DESC";
                // Ejecuta la consulta y devuelve la lista de órdenes
                return connection.Query<Order>(querySelect, new { UserId = userId });
            }
        }

        // Método para obtener detalles de órdenes con información de productos, pagos y usuarios
        public IEnumerable<OrderDetailsViewModel> GetOrderDetails()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                // Consulta para seleccionar detalles de órdenes con información completa de productos, pagos y usuarios
                var query = @"
                    SELECT 
                        o.OrderId,
                        o.OrderNo,
                        o.OrderDate,
                        o.Status,
                        o.Amount,
                        o.PaymentId,
                        u.Name AS UserName,
                        u.UserId,
                        od.ProductId,
                        p.Name AS ProductName,
                        od.Quantity,
                        p.Price,
                        pm.PaymentMode
                    FROM Orders o
                    INNER JOIN OrderDetails od ON o.OrderId = od.OrderId
                    INNER JOIN Products p ON od.ProductId = p.ProductId
                    INNER JOIN Payments pm ON o.PaymentId = pm.PaymentId
                    INNER JOIN Users u ON o.UserId = u.UserId";

                // Diccionario para almacenar las órdenes y sus detalles
                var orderDictionary = new Dictionary<int, OrderDetailsViewModel>();

                // Ejecuta la consulta, mapea los resultados a OrderDetailsViewModel y ProductDetailsViewModel
                var result = connection.Query<OrderDetailsViewModel, ProductDetailsViewModel, OrderDetailsViewModel>(
                    query,
                    (order, product) =>
                    {
                        // Si la orden no está en el diccionario, añádela
                        if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                        {
                            currentOrder = order;
                            currentOrder.Products = new List<ProductDetailsViewModel>();
                            orderDictionary.Add(currentOrder.OrderId, currentOrder);
                        }

                        // Si el producto no es nulo, añádelo a la lista de productos de la orden
                        if (product != null)
                        {
                            currentOrder.Products.Add(product);
                        }

                        return currentOrder; // Devuelve la orden actualizada
                    },
                    splitOn: "ProductName", // Indica el nombre de la columna en la consulta que indica el inicio de la segunda entidad
                    param: new { }
                ).Distinct().ToList(); // Devuelve una lista de órdenes distintas

                return result; // Retorna la lista de órdenes con detalles
            }
        }

        // Método para obtener detalles de órdenes para un usuario específico
        public IEnumerable<OrderDetailsViewModel> GetOrderDetailsById(int userId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                // Consulta para seleccionar detalles de órdenes para un usuario específico, ordenadas por fecha de la orden
                var query = @"
                    SELECT 
                        o.OrderId,
                        o.OrderNo,
                        o.OrderDate,
                        o.Status,
                        o.Amount,
                        o.PaymentId,
                        pm.PaymentMode,
                        u.Name AS UserName,
                        p.Name AS ProductName,
                        od.Quantity,
                        p.Price
                    FROM Orders o
                    INNER JOIN OrderDetails od ON o.OrderId = od.OrderId
                    INNER JOIN Products p ON od.ProductId = p.ProductId
                    INNER JOIN Payments pm ON o.PaymentId = pm.PaymentId
                    INNER JOIN Users u ON o.UserId = u.UserId
                    WHERE o.UserId = @UserId
                    ORDER BY o.OrderDate DESC";

                // Diccionario para almacenar las órdenes y sus detalles
                var orderDictionary = new Dictionary<int, OrderDetailsViewModel>();

                // Ejecuta la consulta, mapea los resultados a OrderDetailsViewModel y ProductDetailsViewModel
                var result = connection.Query<OrderDetailsViewModel, ProductDetailsViewModel, OrderDetailsViewModel>(
                    query,
                    (order, product) =>
                    {
                        // Si la orden no está en el diccionario, añádela
                        if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                        {
                            currentOrder = order;
                            currentOrder.Products = new List<ProductDetailsViewModel>();
                            orderDictionary.Add(currentOrder.OrderId, currentOrder);
                        }

                        // Si el producto no es nulo, añádelo a la lista de productos de la orden
                        if (product != null)
                        {
                            currentOrder.Products.Add(product);
                        }

                        return currentOrder; // Devuelve la orden actualizada
                    },
                    splitOn: "ProductName", // Indica el nombre de la columna en la consulta que indica el inicio de la segunda entidad
                    param: new { UserId = userId } // Parámetro para filtrar las órdenes por UserId
                ).Distinct().ToList(); // Devuelve una lista de órdenes distintas

                return result; // Retorna la lista de órdenes con detalles para el usuario específico
            }
        }
    }
}