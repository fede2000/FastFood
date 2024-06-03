using Dapper;
using FastFood.Models;
using Microsoft.Data.SqlClient;

namespace FastFood.Rules
{
	public class CategoryRule
	{
		private readonly IConfiguration _configuration;
		public CategoryRule(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public List<Category> GetCategories()
		{
			var connectionString = _configuration.GetConnectionString("FastFoodDB");
			using var connection = new SqlConnection(connectionString);
			{
				connection.Open();
				var posts = connection.Query<Category>("SELECT TOP 4 * FROM Categories ORDER BY CreateDate DESC");

				return posts.ToList();
			}
		}
		public void InsertCategory(Category data)
		{
			var connectionString = _configuration.GetConnectionString("FastFoodDB");
			using var connection = new SqlConnection(connectionString);
			{
				connection.Open();

				var queryInsert = "INSERT INTO Categories(Name, ImageUrl, IsActive, CreateDate) Values(@Name, @ImageUrl, @IsActive, @CreateDate)";
				var result = connection.Execute(queryInsert, new
				{
					Name = data.Name,
					ImageUrl = data.ImageUrl,
					IsActive = data.IsActive,
					CreateDate = data.CreateDate,
				});
			}
		}
	}
}
