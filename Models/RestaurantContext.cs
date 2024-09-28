using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EtteremProject.Models
{
    public class RestaurantContext : DbContext
    {
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Meals> Meals { get; set; }
        public DbSet<MealIngredients> MealIngredients { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string databasePath = Path.GetFullPath("restaurant.db");
            MessageBox.Show(databasePath);
			optionsBuilder.UseSqlite($"Data Source={databasePath}");
		}
	}
    public class Orders
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }
    }

    public class Meals
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class MealIngredients
    {
        [Key]
        public int Id { get; set; }
        public int MealId { get; set; }
        public int IngredientId { get; set; }
        public int Quantity { get; set; }
    }

    public class Ingredients
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockAmount { get; set; }
    }

    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int MealId { get; set; }
        public int Quantity { get; set; }
    }
}
