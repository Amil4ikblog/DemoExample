using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DemoExample
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Client> Clients = new ObservableCollection<Client>();
        private string connectionString = "server=127.0.0.1;port=3306;user id=root;password=root;database=demoexample;";

        public MainWindow()
        {
            InitializeComponent();
            Clients = new ObservableCollection<Client>();
            this.DataContext = this; // Установка контекста данных для привязки
            Client.ItemsSource = Clients; // Установка источника данных для ListView
            LoadClients(); // Вызов метода загрузки данных
        }


        private void LoadClients()
        {
            Clients.Clear(); // Очистка списка клиентов перед загрузкой
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT * FROM `clients`", connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                Client client = new Client
                                {
                                    Id_order = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    Surname = reader.GetString("Surname"),
                                    Email = reader.GetString("Email"),
                                    Phone_number = reader.GetString("Phone_number")
                                };
                                Clients.Add(client);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при чтении данных: {ex.Message}");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Проверка, загружены ли клиенты
            if (Clients.Count == 0)
            {
                MessageBox.Show("Нет данных для отображения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string email = EmailTextBox.Text;
            string phoneNumber = PhoneTextBox.Text;

            if (!string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(surname) &&
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(phoneNumber))
            {
                AddClientToDatabase(name, surname, email, phoneNumber);
                LoadClients(); // Обновляем список клиентов после добавления
                ClearInputFields(); // Очищаем текстовые поля после добавления
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearInputFields()
        {
            NameTextBox.Text = string.Empty;
            SurnameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
        }

        private void AddClientToDatabase(string name, string surname, string email, string phoneNumber)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("INSERT INTO `clients` (Name, Surname, Email, Phone_number) VALUES (@Name, @Surname, @Email, @Phone_number)", connection);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone_number", phoneNumber);
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}