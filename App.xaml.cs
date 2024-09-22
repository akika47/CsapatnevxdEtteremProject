using EtteremProject.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EtteremProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
	
    public partial class App : Application
    {
		IServiceCollection services;

		public App()
		{
			services = new ServiceCollection();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			services.AddDbContext<RestaurantContext>();
			services.AddSingleton<MainWindow>();
			var provider = services.BuildServiceProvider();
			provider.GetRequiredService<RestaurantContext>().Database.EnsureCreated();
			provider.GetRequiredService<MainWindow>().Show();
		}
	}

}
