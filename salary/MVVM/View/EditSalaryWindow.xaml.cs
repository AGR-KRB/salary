using System.Windows;
using salary.MVVM.Model;

namespace salary.MVVM.View
{
    public partial class EditSalaryWindow : Window
    {
        private Employee _employee;

        public EditSalaryWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            DataContext = _employee;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _employee.Recalculate(); // Пересчитываем и сохраняем данные в Excel
            DialogResult = true; // Закрываем окно с результатом "True"
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрываем окно без изменений
            Close();
        }
    }
}
