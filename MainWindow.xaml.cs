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
using System.IO;

namespace EtteremProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private RestaurantContext _context;

		private const int HashSize = 256;
		public MainWindow(RestaurantContext context)
		{
			InitializeComponent();

			_context = context;

		}


		// Registration
		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Password;
			Order orderWindow = new Order(_context);

			if (IsValidInput(username, password))
			{

				var user = new Users { Username = username, Password = HashPassword(password) };
				_context.Users.Add(user);
				_context.SaveChanges();
				foreach (var item in _context.Users)
				{
                    this.Close();
                    orderWindow.ShowDialog();
                }

			}
			else
			{
				MessageBox.Show("Hiba történt a regisztrációban!");
			}
		}

		// Login
		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Password;
			Order orderWindow = new Order(_context);


			if (IsValidInput(username, password))
			{


				var user = _context.Users.FirstOrDefault(u => u.Username == username);

				if ((user != null && VerifyPassword(password, user.Password) && user.IsAdmin == true))
				{
					AdminPage adminPage = new AdminPage(_context);
					this.Close();
					adminPage.ShowDialog();

				}
				else if (user != null && VerifyPassword(password, user.Password))
				{
					this.Close();
					orderWindow.ShowDialog();
				}
				else
				{
					MessageBox.Show("Hiba történt a bejelentkezésben!");
				}

			}
			else
			{
				MessageBox.Show("Hiba történt a bejelentkezésben!");
			}
		}

		// Helper methods
		private bool IsValidInput(string username, string password)
		{
			string usernameErrorMessage = "";
			string passwordErrorMessage = "";

			bool errorWithUsername = false;
			bool errorWithPassword = false;


			if (string.IsNullOrWhiteSpace(username))
			{
				usernameErrorMessage = "Nem adtál meg felhasználónevet!";
				errorWithUsername = true;
			}

			if (username.Length < 3)
			{
				usernameErrorMessage = "A felhasználónévnek legalább 3 karakter hosszúnak kell lennie";
				errorWithUsername = true;

			}


			if (string.IsNullOrWhiteSpace(password))
			{
				passwordErrorMessage = "Nem adtál meg jelszót!";
				errorWithPassword = true;
			}

			if (password.Length < 8)
			{
				passwordErrorMessage = "A jelszónak legalább 8 karakter hosszúnak kell lennie!";
				errorWithPassword = true;
			}

			if (password != txtConfirmPassword.Password && txtConfirmPassword.Visibility == Visibility.Visible)
			{
				passwordErrorMessage = "A jelszó vagy a megerősítése téves!";
				errorWithPassword = true;
			}

			if (!password.Any(c => char.IsUpper(c)) || !password.Any(c => char.IsLower(c)) || !password.Any(c => char.IsDigit(c)))
			{
				passwordErrorMessage = "A jelszónak tartalmaznia kell minimum 1 nagybetűs karaktert, 1 kisbetűs karaktert és 1 számot!";
				errorWithPassword = true;
				txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
			}


			if (errorWithUsername && errorWithPassword)
			{
				txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
				txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);
				MessageBox.Show($"{usernameErrorMessage}\n \n{passwordErrorMessage}");
                return false;
            }
            else if (errorWithPassword)
			{
				txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
				MessageBox.Show(passwordErrorMessage);
                return false;
            }
			else if (errorWithUsername)
			{
                txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);
				MessageBox.Show(usernameErrorMessage);
				return false;
            }
			else
			{
                return true;
            }               
		}

		private string HashPassword(string password)
		{
			var bcrypt = new BCryptPasswordEncoder();
			return bcrypt.HashPassword(password);
		}

		public class BCryptPasswordEncoder
		{
			private const int SaltSize = 128;


			public string HashPassword(string password)
			{
				var salt = new byte[SaltSize];
				using (var rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(salt);
				}

				var hash = new byte[HashSize];
				using (var bcrypt = new Rfc2898DeriveBytes(password, salt, 1000))
				{
					hash = bcrypt.GetBytes(HashSize);
				}

				var saltBase64 = Convert.ToBase64String(salt);
				var hashBase64 = Convert.ToBase64String(hash);

				return $"{saltBase64}:{hashBase64}";
			}

		}
		private bool VerifyPassword(string inputPassword, string storedPassword)
		{
			var parts = storedPassword.Split(':');
			if (parts.Length != 2)
			{
				throw new ArgumentException("Invalid stored password format", nameof(storedPassword));
			}

			var saltBase64 = parts[0];
			var hashBase64 = parts[1];

			var salt = Convert.FromBase64String(saltBase64);
			var hash = Convert.FromBase64String(hashBase64);

			byte[] inputHash;

			using (var bcrypt = new Rfc2898DeriveBytes(inputPassword, salt, 1000))
			{
				inputHash = bcrypt.GetBytes(HashSize);
			}

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

        private void RemoveErrorMarker(object sender, RoutedEventArgs e)
        {
			if (sender is TextBox)
			{
				TextBox xd = sender as TextBox;
				xd.BorderBrush = new SolidColorBrush(Colors.Black);
            }
			else 
			{ 
				PasswordBox xd = sender as PasswordBox;
                xd.BorderBrush = new SolidColorBrush(Colors.Black);
            }
			
        }


    }
}