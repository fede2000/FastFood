using Dapper; // Importa Dapper, una biblioteca ORM para facilitar el acceso a la base de datos
using FastFood.Models;
using Microsoft.Data.SqlClient; // Importa el espacio de nombres para trabajar con SQL Server

namespace FastFood.Rules
{
    // Define la clase ProductRule que contiene métodos para manejar operaciones relacionadas con productos
    public class ProductRule
    {
        private readonly IConfiguration _configuration; // Almacena la configuración de la aplicación

        // Constructor que inicializa la configuración
        public ProductRule(IConfiguration configuration)
        {
            _configuration = configuration; // Asigna la configuración pasada al campo _configuration
        }

        // Método para obtener todos los productos ordenados por la fecha de creación en orden descendente
        public List<Product> GetProducts()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar todos los productos ordenados por CreatedDate en orden descendente
                var products = connection.Query<Product>("SELECT * FROM Products ORDER BY CreatedDate DESC");

                return products.ToList(); // Convierte el resultado en una lista y la devuelve
            }
        }

        // Método para insertar un nuevo producto en la base de datos
        public void InsertProduct(Product data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para insertar un nuevo producto en la tabla Products
                var queryInsert = "INSERT INTO Products(Name, Description, Price, Quantity, ImageUrl, CategoryId, IsActive, CreatedDate) Values(@Name, @Description, @Price, @Quantity, @ImageUrl, @CategoryId ,@IsActive, @CreatedDate)";
                // Ejecuta la consulta de inserción usando los valores del objeto data
                var result = connection.Execute(queryInsert, new
                {
                    Name = data.Name,
                    Description = data.Description,
                    Price = data.Price,
                    Quantity = data.Quantity,
                    ImageUrl = data.ImageUrl,
                    CategoryId = data.CategoryId,
                    IsActive = data.IsActive,
                    CreatedDate = data.CreatedDate
                });
            }
        }

        // Método para obtener un producto por su ID
        public Product GetProductById(int productId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar un producto específico basado en el ID
                var query = "SELECT * FROM Products WHERE ProductId = @productId";
                // Ejecuta la consulta y devuelve el producto correspondiente, o null si no se encuentra
                var product = connection.QueryFirstOrDefault<Product>(query, new { productId });

                return product; // Devuelve el producto encontrado
            }
        }

        // Método para actualizar la información de un producto existente
        public void UpdateProduct(Product data)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para actualizar la información de un producto existente
                var queryUpdate = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, Quantity = @Quantity, ImageUrl = @ImageUrl, CategoryId = @CategoryId, IsActive = @IsActive WHERE ProductId = @ProductId";
                // Ejecuta la consulta de actualización usando los valores del objeto data
                var result = connection.Execute(queryUpdate, new
                {
                    Name = data.Name,
                    Description = data.Description,
                    Price = data.Price,
                    Quantity = data.Quantity,
                    ImageUrl = data.ImageUrl,
                    CategoryId = data.CategoryId,
                    IsActive = data.IsActive,
                    ProductId = data.ProductId
                });
            }
        }

        // Método para eliminar un producto por su ID
        public void DeleteProduct(int productId)
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para eliminar un producto basado en el ID
                var queryDelete = "DELETE FROM Products WHERE ProductId = @ProductId";
                // Ejecuta la consulta de eliminación usando el ID del producto
                var result = connection.Execute(queryDelete, new { ProductId = productId });
            }
        }

        // Método para obtener todos los productos activos ordenados por la fecha de creación en orden descendente
        public List<Product> GetActiveProducts()
        {
            var connectionString = _configuration.GetConnectionString("FastFoodDB"); // Obtiene la cadena de conexión de la configuración
            using var connection = new SqlConnection(connectionString); // Crea una nueva conexión a la base de datos
            {
                connection.Open(); // Abre la conexión a la base de datos

                // Consulta para seleccionar productos que están activos, ordenados por CreatedDate en orden descendente
                var products = connection.Query<Product>("SELECT * FROM Products WHERE IsActive = 1 ORDER BY CreatedDate DESC");

                return products.ToList(); // Convierte el resultado en una lista y la devuelve
            }
        }
    }
}
