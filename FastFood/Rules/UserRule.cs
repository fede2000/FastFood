using Dapper; // Importa Dapper para la interacción con la base de datos
using FastFood.Models; // Importa el espacio de nombres que contiene los modelos de la aplicación
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server

namespace FastFood.Rules
{
    // Define la clase UserRule para manejar operaciones relacionadas con usuarios
    public class UserRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la instancia de UserRule con la configuración de la aplicación
        public UserRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para obtener la lista de todos los usuarios, ordenados por la fecha de creación en orden descendente
        public List<User> GetUsers()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para seleccionar todos los usuarios y ordenarlos por CreatedDate en orden descendente
                var users = connection.Query<User>("SELECT * FROM Users ORDER BY CreatedDate DESC");

                // Convierte el resultado de la consulta en una lista y la devuelve
                return users.ToList();
            }
        }

        // Método para validar las credenciales de un usuario
        public User ValidateUser(string email, string password)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para validar las credenciales del usuario comparando el correo electrónico y la contraseña hash
                var query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                // Ejecuta la consulta y obtiene el usuario que coincide con el correo electrónico y la contraseña hash
                var user = connection.QuerySingleOrDefault<User>(query, new { Email = email, Password = HashPassword(password) });

                // Devuelve el usuario encontrado o null si no se encuentra
                return user;
            }
        }

        // Método para obtener un usuario por correo electrónico
        public User GetUsuarioFromEmail(string email)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
                using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
                {
                    connection.Open(); // Abre la conexión a la base de datos

                    // Define la consulta SQL para seleccionar un usuario por su correo electrónico
                    var query = "SELECT * FROM Users WHERE Email = @Email";
                    // Ejecuta la consulta y obtiene el usuario con el correo electrónico proporcionado
                    var user = connection.QuerySingleOrDefault<User>(query, new { Email = email });

                    // Devuelve el usuario encontrado o null si no se encuentra
                    return user;
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción y lanza una excepción de aplicación con el mensaje de error
                throw new ApplicationException("Problema desconocido", ex);
            }
        }

        // Método para validar la contraseña de un usuario comparándola con la contraseña almacenada
        public bool ValidarPassword(User user, string password)
        {
            var hashedPassword = HashPassword(password); // Genera el hash de la contraseña proporcionada
            // Compara la contraseña hash proporcionada con la contraseña hash almacenada en el usuario
            return user.Password == hashedPassword;
        }

        // Método para crear un nuevo usuario en la base de datos
        public void CreateUser(User data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para insertar un nuevo usuario en la tabla Users
                var queryInsert = "INSERT INTO Users(Name, UserName, Mobile, Email, Address, PostCode, Password, ImageUrl, CreatedDate, TipoUsuario) Values(@Name, @UserName, @Mobile, @Email, @Address, @PostCode, @Password, @ImageUrl, @CreatedDate, @TipoUsuario)";
                // Ejecuta la consulta de inserción con los parámetros proporcionados
                var result = connection.Execute(queryInsert, new
                {
                    Name = data.Name,
                    UserName = data.UserName,
                    Mobile = data.Mobile,
                    Email = data.Email,
                    Address = data.Address,
                    PostCode = data.PostCode,
                    ImageUrl = data.ImageUrl,
                    Password = data.Password,
                    CreatedDate = data.CreatedDate,
                    TipoUsuario = TipoUsuario.Usuario // Asigna un tipo de usuario por defecto
                });
            }
        }

        // Método para obtener un usuario por su ID
        public User GetUserById(int userId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para seleccionar un usuario por su ID
                var query = "SELECT * FROM Users WHERE UserId = @userId";
                // Ejecuta la consulta y obtiene el usuario con el ID proporcionado
                var user = connection.QueryFirstOrDefault<User>(query, new { userId });

                // Devuelve el usuario encontrado o null si no se encuentra
                return user;
            }
        }

        // Método para actualizar los detalles de un usuario existente en la base de datos
        public void UpdateUser(User data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para actualizar los detalles de un usuario en la tabla Users
                var queryUpdate = "UPDATE Users SET UserName = @UserName, Mobile = @Mobile, Email = @Email, Address = @Address, PostCode = @PostCode, ImageUrl = @ImageUrl WHERE UserId = @UserId";
                // Ejecuta la consulta de actualización con los parámetros proporcionados
                var result = connection.Execute(queryUpdate, new
                {
                    UserId = data.UserId,
                    UserName = data.UserName,
                    Mobile = data.Mobile,
                    Email = data.Email,
                    Address = data.Address,
                    PostCode = data.PostCode,
                    ImageUrl = data.ImageUrl,
                });
            }
        }

        // Método para eliminar un usuario por su ID
        public void DeleteUser(int userId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión desde la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Define la consulta SQL para eliminar un usuario de la tabla Users
                var queryDelete = "DELETE FROM Users WHERE UserId = @UserId";
                // Ejecuta la consulta de eliminación con el ID del usuario proporcionado
                var result = connection.Execute(queryDelete, new { UserId = userId });
            }
        }

        // Método para hashear una contraseña usando SHA-256
        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create()) // Crea una instancia de SHA-256
            {
                // Convierte la contraseña en un array de bytes y calcula su hash
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Convierte el hash en una cadena hexadecimal y la devuelve en minúsculas
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}