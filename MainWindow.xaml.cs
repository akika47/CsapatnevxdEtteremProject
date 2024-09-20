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
        private RestaurantContext _context;
        public MainWindow()
        {
            InitializeComponent();

            _context = new RestaurantContext("Data Source=restaurant.db");
        }
        // Registration
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;


            // Validate input data...
            if (IsValidInput(username, password))
            {
                using (var context = new RestaurantContext("Data Source=restaurant.db"))
                {
                    var user = new User { Username = username, Password = HashPassword(password) };
                    context.Users.Add(user);
                    context.SaveChanges();
                    // Registration successful, redirect to login or main page...
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
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;


            // Validate input data...
            if (IsValidInput(username, password))
            {
                using (var context = new RestaurantContext("Data Source=restaurant.db"))
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == username);
                    if (user != null && VerifyPassword(password, user.Password))
                    {
                        //Success message later implement
                    }
                    else
                    {
                        //Error message later implement
                    }
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
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                return false;
            }

            // Check if password is not empty and has a minimum length of 8 characters
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            // Check if password contains at least one uppercase letter, one lowercase letter, and one digit
            if (!password.Any(c => char.IsUpper(c)) || !password.Any(c => char.IsLower(c)) || !password.Any(c => char.IsDigit(c)))
            {
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
            private const int HashSize = 256;

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

        }

    }
}