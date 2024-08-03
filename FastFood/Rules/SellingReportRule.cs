using Dapper; // Importa Dapper, una biblioteca ORM para facilitar el acceso a la base de datos
using FastFood.Models.SellingReport; // Importa el espacio de nombres que contiene el modelo SellingReport
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server

namespace FastFood.Rules
{
    // Define la clase SellingReportRule que contiene métodos para manejar operaciones relacionadas con informes de ventas
    public class SellingReportRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public SellingReportRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para obtener un informe de ventas entre dos fechas específicas
        public List<SellingReport> GetReport(DateTime fromDate, DateTime toDate)
        {
            // Valida la fecha de inicio y ajusta si es anterior a la fecha mínima permitida
            if (fromDate < new DateTime(1753, 1, 1))
            {
                fromDate = new DateTime(1753, 1, 1); // Ajusta a la fecha mínima válida para SQL Server
            }

            // Valida la fecha de fin y ajusta si es anterior a la fecha mínima permitida
            if (toDate < new DateTime(1753, 1, 1))
            {
                toDate = new DateTime(1753, 1, 1); // Ajusta a la fecha mínima válida para SQL Server
            }

            // Valida la fecha de fin y ajusta si es posterior a la fecha actual
            if (toDate > DateTime.Now)
            {
                toDate = DateTime.Now; // Ajusta a la fecha actual si es mayor que ahora
            }

            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar un informe de ventas entre dos fechas específicas
                var query = @"
                    SELECT 
                        ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [SrNo], // Genera un número de fila secuencial
                        u.Name, // Nombre del usuario
                        u.Email, // Correo electrónico del usuario
                        COUNT(od.OrderDetailId) AS TotalOrders, // Contador de detalles de órdenes
                        SUM(od.Quantity * od.Price) AS TotalPrice // Suma del precio total de los productos
                    FROM Orders o
                    INNER JOIN OrderDetails od ON o.OrderId = od.OrderId // Une con detalles de órdenes
                    INNER JOIN Products p ON p.ProductId = od.ProductId // Une con productos
                    INNER JOIN Users u ON u.UserId = o.UserId // Une con usuarios
                    WHERE CAST(o.OrderDate AS DATE) BETWEEN @FromDate AND @ToDate // Filtra por rango de fechas
                    GROUP BY u.Name, u.Email"; // Agrupa los resultados por nombre y correo electrónico del usuario

                // Ejecuta la consulta y convierte el resultado en una lista de informes de ventas
                var reportData = connection.Query<SellingReport>(query, new { FromDate = fromDate, ToDate = toDate }).ToList();

                return reportData; // Devuelve la lista de informes de ventas
            }
        }

        // Método para obtener todos los informes de ventas sin filtrado por fecha
        public List<SellingReport> GetAllReports()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar todos los informes de ventas
                var query = @"
                    SELECT 
                        ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [SrNo], // Genera un número de fila secuencial
                        u.Name, // Nombre del usuario
                        u.Email, // Correo electrónico del usuario
                        COUNT(od.OrderDetailId) AS TotalOrders, // Contador de detalles de órdenes
                        SUM(od.Quantity * od.Price) AS TotalPrice // Suma del precio total de los productos
                    FROM Orders o
                    INNER JOIN OrderDetails od ON o.OrderId = od.OrderId // Une con detalles de órdenes
                    INNER JOIN Products p ON p.ProductId = od.ProductId // Une con productos
                    INNER JOIN Users u ON u.UserId = o.UserId // Une con usuarios
                    GROUP BY u.Name, u.Email"; // Agrupa los resultados por nombre y correo electrónico del usuario

                // Ejecuta la consulta y convierte el resultado en una lista de informes de ventas
                return connection.Query<SellingReport>(query).ToList();
            }
        }
    }
}