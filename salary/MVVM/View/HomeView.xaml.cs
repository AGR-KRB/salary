using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OfficeOpenXml;
using salary.MVVM.Model;

namespace salary.MVVM.View
{
    public partial class HomeView : UserControl
    {
        private static readonly string FilePath = "employees.xlsx";
        private List<Employee> _employees; // Список сотрудников для текущего листа
        private string _currentSheet;      // Текущий выбранный лист
            
        public HomeView()
        {
            InitializeComponent();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            _employees = EmployeeRepository.LoadEmployees(); // Загружаем данные сотрудников (общие)
            employeeDataGrid.ItemsSource = _employees;

            // Загружаем список листов из Excel и заполняем ComboBox
            LoadSheetNames();
        }

        private void LoadSheetNames()
        {
            if (!File.Exists(FilePath)) return;

            using (var package = new ExcelPackage(new FileInfo(FilePath)))
            {
                var sheetNames = package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
                SheetSelector.ItemsSource = sheetNames;

                // Выбираем первый лист по умолчанию, если есть
                if (sheetNames.Count > 0)
                {
                    SheetSelector.SelectedIndex = 0;
                }
            }
        }

        private void LoadSalaryData(string sheetName)
        {
            if (_currentSheet != null && _employees != null)
            {
                // Сохраняем изменения текущего листа перед переключением
                SaveSalaryData(_currentSheet);
            }

            // Загружаем данные для выбранного листа
            _employees = EmployeeRepository.LoadEmployeesForMonth(sheetName);
            salaryDataGrid.ItemsSource = _employees;
            _currentSheet = sheetName;
        }

        private void SaveSalaryData(string sheetName)
        {
            // Сохраняем текущие данные в лист Excel
            EmployeeRepository.SaveEmployeesToSheet(_employees, sheetName);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton?.Tag != null)
            {
                if (radioButton.Tag.ToString() == "EmployeeView")
                {
                    employeeDataGrid.Visibility = Visibility.Visible;
                    salaryDataGrid.Visibility = Visibility.Collapsed;
                    SheetSelector.Visibility = Visibility.Collapsed;
                }
                else if (radioButton.Tag.ToString() == "SalaryView")
                {
                    employeeDataGrid.Visibility = Visibility.Collapsed;
                    salaryDataGrid.Visibility = Visibility.Visible;
                    SheetSelector.Visibility = Visibility.Visible;
                }
            }
        }

        private void SalaryDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (salaryDataGrid.SelectedItem is Employee selectedEmployee)
            {
                EditSalaryWindow editWindow = new EditSalaryWindow(selectedEmployee);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Обновляем данные в DataGrid после редактирования
                    salaryDataGrid.Items.Refresh();
                }
            }
        }

        private void SheetSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SheetSelector.SelectedItem != null)
            {
                string selectedSheet = SheetSelector.SelectedItem.ToString();
                LoadSalaryData(selectedSheet);
            }
        }
    }
}
