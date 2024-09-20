using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtteremProject.Models
{
    public class RestaurantContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealIngredient> MealIngredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public RestaurantContext(string connectionString)
         : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions<RestaurantContext> GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder<RestaurantContext>()
                .UseSqlite(connectionString)
                .Options;
        }
    }
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }
    }

    public class Meal
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class MealIngredient
    {
        [Key]
        public int Id { get; set; }
        public int MealId { get; set; }
        public int IngredientId { get; set; }
        public int Quantity { get; set; }
    }

    public class Ingredient
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
