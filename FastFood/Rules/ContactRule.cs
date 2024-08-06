using Dapper; // Importa Dapper, una biblioteca ORM para realizar consultas a bases de datos de manera sencilla
using FastFood.Models;
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server

namespace FastFood.Rules
{
    // Define la clase ContactRule que maneja las reglas de negocio relacionadas con los contactos
    public class ContactRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public ContactRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para agregar una nueva consulta de contacto
        public void AddQuery(Contact data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                // Define la consulta SQL para insertar una nueva entrada en la tabla Contact
                var queryInsert = @"
                    INSERT INTO ConTact (Name, Phone, Email, Subject, Message, CreatedDate) 
                    VALUES (@Name, @Phone, @Email, @Subject, @Message, @CreatedDate)";

                // Ejecuta la consulta SQL con los parámetros proporcionados
                var result = connection.Execute(queryInsert, new
                {
                    Name = data.Name, // Asigna el nombre del contacto
                    Phone = data.Phone, // Asigna el teléfono del contacto
                    Email = data.Email, // Asigna el correo electrónico del contacto
                    Subject = data.Subject, // Asigna el asunto del contacto
                    Message = data.Message, // Asigna el mensaje del contacto
                    CreatedDate = data.CreatedDate // Asigna la fecha de creación de la consulta
                });
            }
        }

        // Método para obtener una consulta de contacto por su ID
        public Contact GetQueryById(int contactId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                                   // Define la consulta SQL para obtener un contacto por su ID
                var query = "SELECT * FROM Contact WHERE ContactId = @ContactId";
                // Ejecuta la consulta y devuelve el primer resultado o null si no se encuentra
                var contact = connection.QueryFirstOrDefault<Contact>(query, new { ContactId = contactId });

                return contact; // Devuelve el contacto obtenido
            }
        }

        // Método para obtener todas las consultas de contacto
        public List<Contact> GetQuerys()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                                   // Define la consulta SQL para obtener todas las consultas de contacto, ordenadas por fecha de creación en orden descendente
                var querys = connection.Query<Contact>("SELECT * FROM Contact ORDER BY CreatedDate DESC");

                return querys.ToList(); // Convierte el resultado de la consulta en una lista y la devuelve
            }
        }

        // Método para eliminar una consulta de contacto por su ID
        public void DeleteQuery(int contactId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                                   // Define la consulta SQL para eliminar un contacto por su ID
                var queryDelete = "DELETE FROM Contact WHERE ContactId = @ContactId";
                // Ejecuta la consulta de eliminación
                var result = connection.Execute(queryDelete, new { ContactId = contactId });
            }
        }
    }
}