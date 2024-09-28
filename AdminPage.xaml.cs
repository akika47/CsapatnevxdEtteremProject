using EtteremProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace EtteremProject
{
	/// <summary>
	/// Interaction logic for AdminPage.xaml
	/// </summary>
	public partial class AdminPage : Window
	{
		private RestaurantContext _context;

		public AdminPage(RestaurantContext context)
		{
			InitializeComponent();
			_context = context;
			LoadIngredients();
		}

		private void LoadIngredients()
		{
			var ingredients = _context.Ingredients.ToList();
			dataGridIngredients.ItemsSource = ingredients;
		}

		private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
		{
			var newIngredient = new Ingredients { Name = txtIngredientName.Text, StockAmount = int.Parse(txtIngredientStock.Text) };
			_context.Ingredients.Add(newIngredient);
			_context.SaveChanges();

			txtIngredientName.Text = "";
			txtIngredientStock.Text = "";
			LoadIngredients();
		}

		private void btnDeleteIngredient_Click(object sender, RoutedEventArgs e)
		{
			var selectedIngredient = dataGridIngredients.SelectedItem as Ingredients;
			if (selectedIngredient != null)
			{
				_context.Ingredients.Remove(selectedIngredient);
				_context.SaveChanges();
				txtIngredientName.Text = "";
				txtIngredientStock.Text = "";
				LoadIngredients();
			}
		}

		private void btnModifyIngredient_Click(object sender, RoutedEventArgs e)
		{
			var selectedIngredient = dataGridIngredients.SelectedItem as Ingredients;
			if (selectedIngredient != null)
			{
				selectedIngredient.Name = txtIngredientName.Text;
				selectedIngredient.StockAmount = int.Parse(txtIngredientStock.Text);
				_context.SaveChanges();
				txtIngredientName.Text = "";
				txtIngredientStock.Text = "";
				LoadIngredients();
			}

		}
		private void dataGridIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedIngredient = dataGridIngredients.SelectedItem as Ingredients;
			if (selectedIngredient != null)
			{
				txtIngredientName.Text = selectedIngredient.Name;
				txtIngredientStock.Text = selectedIngredient.StockAmount.ToString();
			}
		}
	}
}