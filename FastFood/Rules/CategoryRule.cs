using Dapper; // Importa Dapper, una biblioteca ORM para realizar consultas a bases de datos de manera sencilla
using FastFood.Models.Category; // Importa el espacio de nombres que contiene el modelo Category
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server

namespace FastFood.Rules
{
    // Define la clase CategoryRule que maneja las reglas de negocio relacionadas con las categorías
    public class CategoryRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public CategoryRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para obtener todas las categorías
        public List<Category> GetCategories()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                var posts = connection.Query<Category>("SELECT * FROM Categories ORDER BY CreatedDate DESC"); // Ejecuta una consulta SQL para obtener todas las categorías, ordenadas por fecha de creación en orden descendente

                return posts.ToList(); // Convierte el resultado de la consulta en una lista y la devuelve
            }
        }

        // Método para insertar una nueva categoría
        public void InsertCategory(Category data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                var queryInsert = "INSERT INTO Categories(Name, ImageUrl, IsActive, CreatedDate) Values(@Name, @ImageUrl, @IsActive, @CreatedDate)"; // Define la consulta SQL para insertar una nueva categoría
                var result = connection.Execute(queryInsert, new
                {
                    Name = data.Name, // Asigna el nombre de la categoría
                    ImageUrl = data.ImageUrl, // Asigna la URL de la imagen de la categoría
                    IsActive = data.IsActive, // Asigna el estado activo/inactivo de la categoría
                    CreatedDate = data.CreatedDate, // Asigna la fecha de creación de la categoría
                });
            }
        }

        // Método para obtener una categoría por su ID
        public Category GetCategoryById(int categoryId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                var query = "SELECT * FROM Categories WHERE CategoryId = @categoryId"; // Define la consulta SQL para obtener una categoría por su ID
                var category = connection.QueryFirstOrDefault<Category>(query, new { categoryId }); // Ejecuta la consulta y devuelve el primer resultado o null si no se encuentra

                return category; // Devuelve la categoría obtenida
            }
        }

        // Método para actualizar una categoría existente
        public void UpdateCategory(Category data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                var queryUpdate = "UPDATE Categories SET Name = @Name, ImageUrl = @ImageUrl, IsActive = @IsActive WHERE CategoryId = @CategoryId"; // Define la consulta SQL para actualizar una categoría existente
                var result = connection.Execute(queryUpdate, new
                {
                    Name = data.Name, // Asigna el nuevo nombre de la categoría
                    ImageUrl = data.ImageUrl, // Asigna la nueva URL de la imagen de la categoría
                    IsActive = data.IsActive, // Asigna el nuevo estado activo/inactivo de la categoría
                    CategoryId = data.CategoryId // Asigna el ID de la categoría que se va a actualizar
                });
            }
        }

        // Método para eliminar una categoría por su ID
        public void DeleteCategory(int categoryId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                var queryDelete = "DELETE FROM Categories WHERE CategoryId = @CategoryId"; // Define la consulta SQL para eliminar una categoría por su ID
                var result = connection.Execute(queryDelete, new { CategoryId = categoryId }); // Ejecuta la consulta de eliminación
            }
        }

        // Método para obtener todas las categorías activas
        public List<Category> GetActiveCategories()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos
                var categories = connection.Query<Category>("SELECT * FROM Categories WHERE IsActive = 1 ORDER BY CreatedDate DESC"); // Ejecuta una consulta SQL para obtener todas las categorías activas, ordenadas por fecha de creación en orden descendente

                return categories.ToList(); // Convierte el resultado de la consulta en una lista y la devuelve
            }
        }
    }
}