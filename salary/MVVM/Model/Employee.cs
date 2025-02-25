using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace salary.MVVM.Model
{
    public class Employee : INotifyPropertyChanged
    {
        private static readonly string FilePath = "employees.xlsx"; // Путь к файлу Excel

        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int WorkHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public decimal Alimony { get; set; }
        public decimal VacationPay { get; set; }
        public decimal SickPay { get; set; }

        public decimal TotalIncome => (WorkHours * HourlyRate) + Bonus + SickPay;

        public decimal TotalDeductions => Deductions + (TotalIncome * 0.34m) + (TotalIncome * 0.01m) + (TotalIncome * 0.006m);

        public decimal NetSalary => TotalIncome - TotalDeductions;

        // Метод для пересчета и сохранения данных
        public void Recalculate()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalDeductions));
            OnPropertyChanged(nameof(NetSalary));

            SaveToExcel(); // Сохранение изменений в файл
        }

        // Метод для сохранения данных сотрудника в Excel
        private void SaveToExcel()
        {
            if (!File.Exists(FilePath)) return;

            using (var package = new ExcelPackage(new FileInfo(FilePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) return;

                // Найдем строку с данными текущего сотрудника по его EmployeeID
                var row = FindRowByEmployeeID(worksheet, EmployeeID);
                if (row != -1)
                {
                    worksheet.Cells[row, 2].Value = Name;
                    worksheet.Cells[row, 3].Value = Position;
                    worksheet.Cells[row, 4].Value = WorkHours;
                    worksheet.Cells[row, 5].Value = HourlyRate;
                    worksheet.Cells[row, 6].Value = Bonus;
                    worksheet.Cells[row, 7].Value = Deductions;
                    worksheet.Cells[row, 8].Value = Alimony;
                    worksheet.Cells[row, 9].Value = VacationPay;
                    worksheet.Cells[row, 10].Value = SickPay;
                    worksheet.Cells[row, 11].Value = TotalIncome;
                    worksheet.Cells[row, 12].Value = TotalDeductions;
                    worksheet.Cells[row, 13].Value = NetSalary;
                }

                package.Save();
            }
        }

        // Метод для поиска строки по EmployeeID
        private int FindRowByEmployeeID(ExcelWorksheet worksheet, string employeeID)
        {
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Предполагается, что первая строка — это заголовки
            {
                if (worksheet.Cells[row, 1].Text == employeeID)
                {
                    return row;
                }
            }

            return -1; // Если не найден
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
