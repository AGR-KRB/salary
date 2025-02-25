using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OfficeOpenXml; // Подключение пространства имен EPPlus
using OfficeOpenXml.Table; // Для создания таблиц Excel
using salary.MVVM.Model;

namespace salary.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
            LoadEmployeeData();
        }

        // Метод для загрузки данных сотрудников и расчета зарплаты
        private void LoadEmployeeData()
        {
            List<Employee> employees = EmployeeRepository.LoadEmployees();
            List<SalaryReport> reports = new List<SalaryReport>();

            foreach (var employee in employees)
            {
                decimal grossSalary = employee.WorkHours * employee.HourlyRate + employee.Bonus + employee.VacationPay + employee.SickPay;
                decimal totalDeductions = employee.Deductions + employee.Alimony;
                decimal netSalary = grossSalary - totalDeductions;

                reports.Add(new SalaryReport
                {
                    EmployeeID = employee.EmployeeID,
                    Name = employee.Name,
                    Position = employee.Position,
                    Salary = netSalary,
                    TotalDeductions = totalDeductions
                });
            }

            ReportDataGrid.ItemsSource = reports;
        }

        // Генерация отчета (например, можно сохранять файл или показывать отчет в UI)
        // Добавьте это пространство имен

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            // Получение данных из DataGrid
            var reports = (List<SalaryReport>)ReportDataGrid.ItemsSource;

            // Создание диалога для выбора файла сохранения
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*", // Фильтр типов файлов
                FileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", // Имя файла по умолчанию
                Title = "Сохранение отчета"
            };

            // Открытие диалога выбора файла
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName; // Получение пути файла

                try
                {
                    // Создание нового Excel файла
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Создаем лист в Excel
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Отчет");

                        // Заголовки таблицы
                        worksheet.Cells[1, 1].Value = "Таб. номер";
                        worksheet.Cells[1, 2].Value = "Имя";
                        worksheet.Cells[1, 3].Value = "Должность";
                        worksheet.Cells[1, 4].Value = "Зарплата";
                        worksheet.Cells[1, 5].Value = "Удержания";

                        // Заполнение данных из отчета
                        for (int i = 0; i < reports.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = reports[i].EmployeeID;
                            worksheet.Cells[i + 2, 2].Value = reports[i].Name;
                            worksheet.Cells[i + 2, 3].Value = reports[i].Position;
                            worksheet.Cells[i + 2, 4].Value = reports[i].Salary;
                            worksheet.Cells[i + 2, 5].Value = reports[i].TotalDeductions;
                        }

                        // Форматирование таблицы
                        var range = worksheet.Cells[1, 1, reports.Count + 1, 5];
                        var table = worksheet.Tables.Add(range, "ReportTable");
                        table.TableStyle = TableStyles.Medium2; // Стиль таблицы

                        worksheet.Cells.AutoFitColumns(); // Автоматическая подгонка ширины столбцов

                        // Сохранение файла Excel
                        package.SaveAs(new FileInfo(fileName));
                    }

                    // Уведомление об успешном сохранении
                    MessageBox.Show($"Отчет успешно сохранен в файл: {fileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Открытие папки с сохраненным файлом
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,{fileName}");
                }
                catch (Exception ex)
                {
                    // Уведомление о возникновении ошибки
                    MessageBox.Show($"Произошла ошибка при сохранении отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Модель для отчета по заработной плате
        public class SalaryReport
        {
            public string EmployeeID { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public decimal Salary { get; set; }
            public decimal TotalDeductions { get; set; }
        }
    }
}
