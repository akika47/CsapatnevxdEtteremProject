using EtteremProject.Models;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EtteremProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        Order orderWindow = new Order();
        private RestaurantContext _context;
		private const int HashSize = 256;
		public MainWindow(RestaurantContext context)
		{
			InitializeComponent();

			_context = context;
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		// Registration
		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Text;


			// Validate input data...
			if (IsValidInput(username, password))
			{

				var user = new Users { Username = username, Password = HashPassword(password) };
				_context.Users.Add(user);
				_context.SaveChanges();
				// Registration successful, redirect to login or main page...
				foreach (var item in _context.Users)
				{
					//MessageBox.Show($"{item.Username} {item.Password}");
                    this.Close();
                    orderWindow.ShowDialog();
                }

			}
			else
			{
				// Display error message...
			}
		}

		// Login
		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Text;




			// Validate input data...
			if (IsValidInput(username, password))
			{


				var user = _context.Users.FirstOrDefault(u => u.Username == username);
				if (user != null && VerifyPassword(password, user.Password))
				{
					this.Close();		
					orderWindow.ShowDialog();
				}
				else
				{
					//Error message later implement
				}

			}
			else
			{
				//Error message later implement
			}
		}

		// Helper methods
		private bool IsValidInput(string username, string password)
		{
			// Check if username is not empty and has a minimum length of 3 characters
			if (string.IsNullOrWhiteSpace(username))
			{
				MessageBox.Show("Nem adtál meg felhasználónevet!");
				txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);

				return false;
			}

			if (username.Length < 3)
			{
				MessageBox.Show("A felhasználónévnek legalább 3 karakter hosszúnak kell lennie");
                txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);

                return false;
			}

			// Check if password is not empty and has a minimum length of 8 characters
			if (string.IsNullOrWhiteSpace(password))
			{
				MessageBox.Show("Nem adtál meg jelszót!");
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);

                return false;
			}

			if (password.Length < 8)
			{
				MessageBox.Show("A jelszónak legalább 8 karakter hosszúnak kell lennie!");
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
			}

			if (password != txtConfirmPassword.Text && txtConfirmPassword.Visibility == Visibility.Visible) 
			{
				MessageBox.Show("A jelszó vagy a megerősítése téves!");
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
				txtConfirmPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
			}

			// Check if password contains at least one uppercase letter, one lowercase letter, and one digit
			if (!password.Any(c => char.IsUpper(c)) || !password.Any(c => char.IsLower(c)) || !password.Any(c => char.IsDigit(c)))
			{
				MessageBox.Show("A jelszónak nagybetűs karaktert is klle tartalmaznia!");
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
			}

			// If all checks pass, input is valid
			return true;
		}

		private string HashPassword(string password)
		{
			// Use bcrypt to hash the password
			var bcrypt = new BCryptPasswordEncoder();
			return bcrypt.HashPassword(password);
		}

		public class BCryptPasswordEncoder
		{
			private const int SaltSize = 128;


			public string HashPassword(string password)
			{
				// Generate a random salt
				var salt = new byte[SaltSize];
				using (var rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(salt);
				}

				// Hash the password using bcrypt
				var hash = new byte[HashSize];
				using (var bcrypt = new Rfc2898DeriveBytes(password, salt, 1000))
				{
					hash = bcrypt.GetBytes(HashSize);
				}

				// Convert the salt and hash to base64 strings
				var saltBase64 = Convert.ToBase64String(salt);
				var hashBase64 = Convert.ToBase64String(hash);

				// Return the hashed password in the format "salt:hash"
				return $"{saltBase64}:{hashBase64}";
			}

		}
		private bool VerifyPassword(string inputPassword, string storedPassword)
		{
			// Split the stored password into salt and hash
			var parts = storedPassword.Split(':');
			if (parts.Length != 2)
			{
				throw new ArgumentException("Invalid stored password format", nameof(storedPassword));
			}

			var saltBase64 = parts[0];
			var hashBase64 = parts[1];

			// Convert the salt and hash back to byte arrays
			var salt = Convert.FromBase64String(saltBase64);
			var hash = Convert.FromBase64String(hashBase64);

			// Declare the inputHash variable before the using statement
			byte[] inputHash;

			// Hash the input password using the same salt and bcrypt algorithm
			using (var bcrypt = new Rfc2898DeriveBytes(inputPassword, salt, 1000))
			{
				inputHash = bcrypt.GetBytes(HashSize);
			}

			// Compare the input hash with the stored hash
			return inputHash.SequenceEqual(hash);
		}

        private void OpenRegisterContext(object sender, RoutedEventArgs e)
        {
			txtConfirmPassword.Visibility = Visibility.Visible;
			lblConfirmPassword.Visibility = Visibility.Visible;

			lblAlreadyHave.Visibility = Visibility.Visible;
			btnAlreadyHave.Visibility = Visibility.Visible;

			lblClickHere.Visibility = Visibility.Collapsed;
			btnClickHere.Visibility = Visibility.Collapsed;

			btnLogin.Visibility = Visibility.Collapsed;
			btnRegister.Visibility = Visibility.Visible;

			lblTitle.Content = "Register";

        }

        private void OpenLoginContext(object sender, RoutedEventArgs e)
        {
            txtConfirmPassword.Visibility = Visibility.Collapsed;
            lblConfirmPassword.Visibility = Visibility.Collapsed;

            lblAlreadyHave.Visibility = Visibility.Collapsed;
            btnAlreadyHave.Visibility = Visibility.Collapsed;

            lblClickHere.Visibility = Visibility.Visible;
            btnClickHere.Visibility = Visibility.Visible;

			btnRegister.Visibility= Visibility.Collapsed;
			btnLogin.Visibility = Visibility.Visible;

			lblTitle.Content = "Login";
        }

        private void asd(object sender, RoutedEventArgs e)
        {
			this.Close();
			Order newpage = new Order();
			newpage.ShowDialog();
        }

        private void RemoveErrorMarker(object sender, RoutedEventArgs e)
        {
			TextBox kuldoTxt = sender as TextBox;

			kuldoTxt.BorderBrush = new SolidColorBrush(Colors.Black);
        }
    }
}