using Dapper; // Importa Dapper para la interacción con la base de datos
using FastFood.Models;
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server
using Microsoft.Extensions.Configuration; // Importa el espacio de nombres para acceder a la configuración de la aplicación

namespace FastFood.Rules
{
    // Define la clase SimplePaymentRule para manejar operaciones relacionadas con pagos
    public class SimplePaymentRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public SimplePaymentRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para insertar un nuevo pago en la base de datos
        public void InsertSimplePayment(Payment payment)
        {
            // Verifica si el objeto payment es nulo y lanza una excepción si es así
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null"); // Lanza una excepción si payment es nulo
            }

            // Obtiene la cadena de conexión desde la configuración
            var connectionString = _configuration.GetConnectionString("FastFoodDB");
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para insertar un nuevo registro en la tabla Payments
                var queryInsert = "INSERT INTO Payments (Address, PaymentMode, Amount) VALUES (@Address, @PaymentMode, @Amount)";
                // Ejecuta la consulta y pasa los parámetros del objeto payment
                connection.Execute(queryInsert, new
                {
                    Address = payment.Address,
                    PaymentMode = payment.PaymentMode,
                    Amount = payment.Amount
                });
            }
        }

        // Método para obtener el ID del último pago registrado en la base de datos
        public int GetLatestPaymentId()
        {
            // Obtiene la cadena de conexión desde la configuración
            var connectionString = _configuration.GetConnectionString("FastFoodDB");
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para seleccionar el ID del último registro en la tabla Payments
                var query = "SELECT TOP 1 PaymentId FROM Payments ORDER BY PaymentId DESC";
                // Ejecuta la consulta y obtiene el valor del PaymentId más reciente
                return connection.QuerySingleOrDefault<int>(query); // Utiliza QuerySingleOrDefault para devolver el valor del primer registro o 0 si no se encuentra
            }
        }
    }
}