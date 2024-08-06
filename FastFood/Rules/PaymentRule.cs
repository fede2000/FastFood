using Dapper; // Importa Dapper, una biblioteca ORM para facilitar el acceso a la base de datos
using FastFood.Models;
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server
using Microsoft.Extensions.Configuration; // Importa el espacio de nombres para manejar la configuración
using System.Data; // Importa el espacio de nombres para trabajar con tipos de datos relacionados con la base de datos
using System.Data.SqlTypes; // Importa el espacio de nombres para tipos de datos específicos de SQL Server
using static System.Runtime.InteropServices.JavaScript.JSType; // Importa tipos específicos de JavaScript (no se utiliza en este contexto)

namespace FastFood.Rules
{
    // Define la clase PaymentRule que contiene métodos para manejar operaciones relacionadas con pagos
    public class PaymentRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public PaymentRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para insertar un nuevo pago en la base de datos
        public void InsertPayment(Payment payment)
        {
            // Verifica si el objeto payment es nulo y lanza una excepción si es así
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null");
            }

            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para insertar un nuevo registro en la tabla Payments
                var queryInsert = "INSERT INTO Payments (Name, CardNo, ExpireDate, Cvv, Address, PaymentMode, Amount) VALUES (@Name, @CardNo, @ExpireDate, @Cvv, @Address, @PaymentMode, @Amount)";
                // Ejecuta la consulta de inserción usando los valores del objeto payment
                var result = connection.Execute(queryInsert, new
                {
                    Name = payment.Name,
                    CardNo = payment.CardNo,
                    ExpireDate = payment.ExpireDate,
                    Cvv = payment.Cvv,
                    Address = payment.Address,
                    PaymentMode = payment.PaymentMode,
                    Amount = payment.Amount
                });
            }
        }

        // Método para obtener el ID del último pago insertado
        public int GetLatestPaymentId()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar el ID del último pago insertado, ordenado por PaymentId en orden descendente
                var query = "SELECT TOP 1 PaymentId FROM Payments ORDER BY PaymentId DESC";
                // Ejecuta la consulta y devuelve el ID del último pago
                return connection.QuerySingleOrDefault<int>(query);
            }
        }
    }
}