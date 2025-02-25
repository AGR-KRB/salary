using salary.MVVM.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
namespace salary.MVVM.View
{
    public partial class WorkersView : UserControl
    {
        private List<Employee> employees = new List<Employee>();
        public WorkersView()
        {
            InitializeComponent(); // Связывает XAML и код
        }

        private void ClearFields()
        {
            txtEmployeeID.Clear();
            txtName.Clear();
            txtPosition.Clear();
            txtWorkHours.Clear();
            txtHourlyRate.Clear();
            txtBonus.Clear();
            txtDeductions.Clear();
            txtAlimony.Clear();
            txtVacationPay.Clear();
            txtSickPay.Clear();
        }
        private void TextBox_OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text, @"^\d+(,\d{0,2})?$");
        }

       
        private void TextBox_OnlyNumbers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; 
            }
        }

        private bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Получаем введенные данные
            Employee newEmployee = new Employee
            {
                EmployeeID = txtEmployeeID.Text, // Табельный номер
                Name = txtName.Text,
                Position = txtPosition.Text,
                WorkHours = int.Parse(txtWorkHours.Text),
                HourlyRate = decimal.Parse(txtHourlyRate.Text),
                Bonus = decimal.Parse(txtBonus.Text),
                Deductions = decimal.Parse(txtDeductions.Text),
                Alimony = decimal.Parse(txtAlimony.Text),
                VacationPay = decimal.Parse(txtVacationPay.Text),
                SickPay = decimal.Parse(txtSickPay.Text)
            };

            // Проверка на уникальность табельного номера
            if (employees.Any(e => e.EmployeeID == newEmployee.EmployeeID))
            {
                MessageBox.Show($"Сотрудник с табельным номером {newEmployee.EmployeeID} уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Добавляем нового сотрудника в список
            employees.Add(newEmployee);

            // Сохраняем в файл
            EmployeeRepository.SaveEmployees(employees);

            MessageBox.Show($"Сотрудник {newEmployee.Name} добавлен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Очищаем поля после добавления сотрудника
            ClearFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
}