using EtteremProject.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using EtteremProject.Models;

namespace EtteremProject
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : Window
    {


		private RestaurantContext _context;

		public Order(RestaurantContext context)
		{
			InitializeComponent();
			_context = context;
		}

		private void OpenLoginWindow(object sender, RoutedEventArgs e)
        {
            MainWindow ujMain = new MainWindow(_context);
            this.Close();
            ujMain.ShowDialog();
            
        }

		private bool CheckIngredients(string mealName, int quantity)
		{
			var meal = _context.Meals.FirstOrDefault(m => m.Name == mealName);
			if (meal == null)
			{
				return false;
			}

			var mealIngredients = _context.MealIngredients.Where(mi => mi.MealId == meal.Id).ToList();
			foreach (var mealIngredient in mealIngredients)
			{
				var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == mealIngredient.IngredientId);
				if (ingredient.StockAmount < mealIngredient.Quantity * quantity)
				{
					return false;
				}
			}

			return true;
		}

		Dictionary<string, int> DishQunatitites = new Dictionary<string, int>();
		private void IncreaseDecrease(object sender, RoutedEventArgs e)
		{
			var gomb = sender as Button;
			var stackpanel = gomb.Parent as StackPanel;
			var label = stackpanel.Children[1] as Label;
			if (gomb.Content.ToString() == "+")
			{
				if (CheckIngredients(stackpanel.Name, Convert.ToInt32(label.Content) + 1))
				{
					label.Content = Convert.ToInt32(label.Content) + 1;
					if (!DishQunatitites.ContainsKey(stackpanel.Name))
					{
						DishQunatitites.Add(stackpanel.Name, 1);
					}
					else
					{
						DishQunatitites[stackpanel.Name] += 1;
					}
				}
				else
				{
					MessageBox.Show("Not enough ingredients to make this meal.");
				}
			}
			else if (gomb.Content.ToString() == "-" && label.Content.ToString() != "0")
			{
				label.Content = Convert.ToInt32(label.Content) - 1;

				DishQunatitites[stackpanel.Name] -= 1;
			}

			lbCart.Items.Clear();
			foreach (var item in DishQunatitites)
			{
				if (item.Value != 0)
					lbCart.Items.Add($"{item.Key}\t \t{item.Value}");
				else
					DishQunatitites.Remove(item.Key);

			}
		}

		private void btnCheckout_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in DishQunatitites)
			{
				var meal = _context.Meals.FirstOrDefault(m => m.Name == item.Key);
				if (meal != null)
				{
					var mealIngredients = _context.MealIngredients.Where(mi => mi.MealId == meal.Id).ToList();
					foreach (var mealIngredient in mealIngredients)
					{
						var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == mealIngredient.IngredientId);
						ingredient.StockAmount -= mealIngredient.Quantity * item.Value;
					}
				}
			}

			_context.SaveChanges();
			DishQunatitites.Clear();
			lbCart.Items.Clear();
		}
	}
}