using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using salary.MVVM.Model;

namespace salary.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для SeasonView.xaml
    /// </summary>
    public partial class SeasonView : UserControl
    {
        public SeasonView()
        {
            InitializeComponent();
        }

        private void AddEmployeesForMonth_Click(object sender, RoutedEventArgs e)
        {
            if (MonthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите месяц.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем выбранную дату и преобразуем ее в строку с названием месяца
            DateTime selectedDate = MonthDatePicker.SelectedDate.Value;
            string month = selectedDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

            // Загрузка данных сотрудников из файла
            List<Employee> employees = EmployeeRepository.LoadEmployees();

            if (employees == null || employees.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения. Пожалуйста, добавьте сотрудников.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Сохранение сотрудников для указанного месяца
                EmployeeRepository.SaveEmployeesForMonth(employees, month);
                MessageBox.Show($"Данные за {month} успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
