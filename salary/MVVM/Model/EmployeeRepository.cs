using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace salary.MVVM.Model
{
    public static class EmployeeRepository
    {

        private static readonly string FilePath = "employees.xlsx";

        /// <summary>
        /// Сохраняет или добавляет новых сотрудников в файл Excel (.xlsx)
        /// </summary>
        /// <param name="employees">Список новых сотрудников</param>
        public static void SaveEmployees(List<Employee> employees)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo fileInfo = new FileInfo(FilePath);

            using (ExcelPackage package = fileInfo.Exists ? new ExcelPackage(fileInfo) : new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets["Employees"];

                // Если листа еще нет, создаем его
                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add("Employees");

                    // Заголовок
                    worksheet.Cells[1, 1].Value = "EmployeeID";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Position";
                    worksheet.Cells[1, 4].Value = "WorkHours";
                    worksheet.Cells[1, 5].Value = "HourlyRate";
                    worksheet.Cells[1, 6].Value = "Bonus";
                    worksheet.Cells[1, 7].Value = "Deductions";
                    worksheet.Cells[1, 8].Value = "Alimony";
                    worksheet.Cells[1, 9].Value = "VacationPay";
                    worksheet.Cells[1, 10].Value = "SickPay";

                    // Форматирование заголовка
                    using (var range = worksheet.Cells[1, 1, 1, 10])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                }

                // Определение следующей свободной строки
                int startRow = worksheet.Dimension?.Rows + 1 ?? 2;

                // Заполнение данными
                for (int i = 0; i < employees.Count; i++)
                {
                    var employee = employees[i];
                    worksheet.Cells[startRow + i, 1].Value = employee.EmployeeID;
                    worksheet.Cells[startRow + i, 2].Value = employee.Name;
                    worksheet.Cells[startRow + i, 3].Value = employee.Position;
                    worksheet.Cells[startRow + i, 4].Value = employee.WorkHours;
                    worksheet.Cells[startRow + i, 5].Value = employee.HourlyRate;
                    worksheet.Cells[startRow + i, 6].Value = employee.Bonus;
                    worksheet.Cells[startRow + i, 7].Value = employee.Deductions;
                    worksheet.Cells[startRow + i, 8].Value = employee.Alimony;
                    worksheet.Cells[startRow + i, 9].Value = employee.VacationPay;
                    worksheet.Cells[startRow + i, 10].Value = employee.SickPay;
                }

                // Автоматическое изменение ширины колонок
                worksheet.Cells.AutoFitColumns();

                // Сохранение файла
                package.Save();
            }
        }
        public static void SaveEmployeesForMonth(List<Employee> employees, string month)
        {
            try
            {
                // Проверка корректного имени листа
                month = GetValidSheetName(month);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(FilePath);

                // Проверка доступности файла
                if (fileInfo.Exists && !IsFileAvailable(fileInfo))
                {
                    throw new IOException("Файл уже открыт другим процессом. Закройте файл и попробуйте снова.");
                }

                using (ExcelPackage package = fileInfo.Exists ? new ExcelPackage(fileInfo) : new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets[month] ?? package.Workbook.Worksheets.Add(month);

                    // Если лист новый, добавляем заголовок
                    if (worksheet.Dimension == null)
                    {
                        worksheet.Cells[1, 1].Value = "EmployeeID";
                        worksheet.Cells[1, 2].Value = "Name";
                        worksheet.Cells[1, 3].Value = "Position";
                        worksheet.Cells[1, 4].Value = "WorkHours";
                        worksheet.Cells[1, 5].Value = "HourlyRate";
                        worksheet.Cells[1, 6].Value = "Bonus";
                        worksheet.Cells[1, 7].Value = "Deductions";
                        worksheet.Cells[1, 8].Value = "Alimony";
                        worksheet.Cells[1, 9].Value = "VacationPay";
                        worksheet.Cells[1, 10].Value = "SickPay";
                        worksheet.Cells[1, 11].Value = "TotalSalary";

                        // Форматирование заголовка
                        using (var range = worksheet.Cells[1, 1, 1, 11])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Определение следующей свободной строки
                    int startRow = worksheet.Dimension?.Rows + 1 ?? 2;

                    // Заполнение данными
                    for (int i = 0; i < employees.Count; i++)
                    {
                        var employee = employees[i];
                        decimal totalSalary = (employee.WorkHours * employee.HourlyRate) + employee.Bonus - employee.Deductions - employee.Alimony + employee.VacationPay + employee.SickPay;

                        worksheet.Cells[startRow + i, 1].Value = employee.EmployeeID;
                        worksheet.Cells[startRow + i, 2].Value = employee.Name;
                        worksheet.Cells[startRow + i, 3].Value = employee.Position;
                        worksheet.Cells[startRow + i, 4].Value = employee.WorkHours;
                        worksheet.Cells[startRow + i, 5].Value = employee.HourlyRate;
                        worksheet.Cells[startRow + i, 6].Value = employee.Bonus;
                        worksheet.Cells[startRow + i, 7].Value = employee.Deductions;
                        worksheet.Cells[startRow + i, 8].Value = employee.Alimony;
                        worksheet.Cells[startRow + i, 9].Value = employee.VacationPay;
                        worksheet.Cells[startRow + i, 10].Value = employee.SickPay;
                        worksheet.Cells[startRow + i, 11].Value = totalSalary;
                    }

                    // Автоматическое изменение ширины колонок
                    worksheet.Cells.AutoFitColumns();

                    // Сохранение файла
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        // Метод для проверки доступности файла
        private static bool IsFileAvailable(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        // Метод для получения корректного имени листа
        private static string GetValidSheetName(string name)
        {
            char[] invalidChars = new char[] { '/', '\\', '*', '[', ']', ':', '?', '\'' };
            foreach (char invalidChar in invalidChars)
            {
                name = name.Replace(invalidChar, '_');
            }

            // Обрезаем длину имени до 31 символа (максимально допустимое значение)
            return name.Length > 31 ? name.Substring(0, 31) : name;
        }

        public static List<Employee> LoadEmployeesForMonth(string month)
        {
            var employees = new List<Employee>();

            if (!File.Exists(FilePath)) return employees;

            using (var package = new ExcelPackage(new FileInfo(FilePath)))
            {
                var worksheet = package.Workbook.Worksheets[month];
                if (worksheet == null) return employees;

                int rows = worksheet.Dimension.Rows;

                for (int i = 2; i <= rows; i++)
                {
                    employees.Add(new Employee
                    {
                        EmployeeID = worksheet.Cells[i, 1].Text,
                        Name = worksheet.Cells[i, 2].Text,
                        Position = worksheet.Cells[i, 3].Text,
                        WorkHours = int.Parse(worksheet.Cells[i, 4].Text),
                        HourlyRate = decimal.Parse(worksheet.Cells[i, 5].Text),
                        Bonus = decimal.Parse(worksheet.Cells[i, 6].Text),
                        Deductions = decimal.Parse(worksheet.Cells[i, 7].Text),
                        Alimony = decimal.Parse(worksheet.Cells[i, 8].Text),
                        VacationPay = decimal.Parse(worksheet.Cells[i, 9].Text),
                        SickPay = decimal.Parse(worksheet.Cells[i, 10].Text)
                    });
                }
            }
            return employees;
        }


        /// <summary>
        /// Загружает список сотрудников из файла Excel (.xlsx)
        /// </summary>
        /// <returns>Список объектов сотрудников</returns>
        public static List<Employee> LoadEmployees()
        {
            var employees = new List<Employee>();

            if (!File.Exists(FilePath))
                return employees;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
            {
                var worksheet = package.Workbook.Worksheets["Employees"];
                if (worksheet == null) return employees;

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Начинаем со 2-й строки, так как 1-я — это заголовок
                {
                    employees.Add(new Employee
                    {
                        EmployeeID = worksheet.Cells[row, 1].Text,
                        Name = worksheet.Cells[row, 2].Text,
                        Position = worksheet.Cells[row, 3].Text,
                        WorkHours = int.Parse(worksheet.Cells[row, 4].Text),
                        HourlyRate = decimal.Parse(worksheet.Cells[row, 5].Text),
                        Bonus = decimal.Parse(worksheet.Cells[row, 6].Text),
                        Deductions = decimal.Parse(worksheet.Cells[row, 7].Text),
                        Alimony = decimal.Parse(worksheet.Cells[row, 8].Text),
                        VacationPay = decimal.Parse(worksheet.Cells[row, 9].Text),
                        SickPay = decimal.Parse(worksheet.Cells[row, 10].Text)
                    });
                }
            }

            return employees;
        }
        public static void SaveEmployeesToSheet(List<Employee> employees, string sheetName)
        {
            if (!File.Exists(FilePath)) return;

            using (var package = new ExcelPackage(new FileInfo(FilePath)))
            {
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null) return;

                int row = 2; // Данные сохраняются с 2-й строки
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.EmployeeID;
                    worksheet.Cells[row, 2].Value = employee.Name;
                    worksheet.Cells[row, 3].Value = employee.Position;
                    worksheet.Cells[row, 4].Value = employee.WorkHours;
                    worksheet.Cells[row, 5].Value = employee.HourlyRate;
                    worksheet.Cells[row, 6].Value = employee.Bonus;
                    worksheet.Cells[row, 7].Value = employee.Deductions;
                    worksheet.Cells[row, 8].Value = employee.Alimony;
                    worksheet.Cells[row, 9].Value = employee.SickPay;
                    worksheet.Cells[row, 10].Value = employee.NetSalary;

                    row++;
                }

                package.Save(); // Сохраняем изменения в файл Excel
            }
        }
    }
}