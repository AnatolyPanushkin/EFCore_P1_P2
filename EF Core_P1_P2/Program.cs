using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EF_Core_P1_P2.Data.Models;

namespace EF_Core_P1_P2
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();

        static void Main(string[] args)
        {
            //Console.WriteLine(GetEmployeesInformation());
            //WellPaidEmployees();
            //MoveEployees();
            //ProjectAudit();
            //DossieOnTheEmployee(1);
            //SmallDepartment();
            //SalaryIncrease("Engineering",10);
            //DeleteDepartment(1);
            DeleteTown("Redmond");
        }

        /// <summary>
        /// Функция вывода на экран всех сотрудников
        /// </summary>
        /// <returns></returns>
        static string GetEmployeesInformation()
        {
            var employees = _context.Employees.OrderBy(e => e.EmployeeId)
                .Select(e => new {e.FirstName, e.LastName, e.MiddleName, e.JobTitle}).ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}");
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Функция выводит сотрудников с зарплатой выше 48 000
        /// </summary>
        static void WellPaidEmployees()
        {
            var employees = _context.Employees.OrderBy(e => e.LastName).Where(e => e.Salary > 48000)
                .Select(e => e).ToList();
            var sb = new StringBuilder();
            foreach (var var in employees)
            {
                sb.AppendLine(
                    $"Id: {var.EmployeeId} Имя: {var.FirstName}, Фамилия: {var.LastName} MidleName:{var.MiddleName} Зарплата:{var.Salary}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        /// <summary>
        /// Функция изменяет адрес сотрудника с фамилией Brown
        /// </summary>
        static void MoveEployees()
        {
            var _address = new Address("LagunaBeach", 1);
            var address = _context.Addresses.Add(_address);
            _context.SaveChanges();

            var employees = _context.Employees.Where(e => e.LastName.Equals("Brown")).Select(e => e).ToList();

            var sb = new StringBuilder();
            foreach (var varEmployee in employees)
            {
                varEmployee.AddressId = _address.AddressId;
                _context.SaveChanges();
                sb.AppendLine(
                    $"{varEmployee.EmployeeId}, {varEmployee.FirstName},{varEmployee.LastName},{varEmployee.MiddleName},{varEmployee.JobTitle},{varEmployee.DepartmentId},{varEmployee.ManagerId},{varEmployee.HireDate},{varEmployee.Salary},{varEmployee.AddressId}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        /// <summary>
        /// Выводим первых 5-ых сотрудников, у которых были проекты в промежутке с 2002 по 2005 год.
        /// </summary>
        static void ProjectAudit()
        {
            var projects = _context.Projects.Where(e => e.StartDate.Year <= 2005 && e.StartDate.Year >= 2002)
                .Select(e => e).ToList();
            var employees = _context.Employees.ToList();
            for (int i = 0; i < projects.Count; i++)
            {
                var employeesProjects = _context.EmployeesProjects.Where(e => e.ProjectId == projects[i].ProjectId)
                    .Select(e => e.Employee).Take(5).ToList();
                employees = employeesProjects;

            }

            for (int j = 0; j < employees.Count; j++)
            {
                Console.WriteLine(
                    $"Сотрудник: {employees[j].FirstName} его менеджер: {employees[j].Manager.FirstName} {employees[j].Manager.LastName}");
                if (projects[j].EndDate != null)
                {
                    Console.WriteLine(
                        $"Проект: {projects[j].Name} дата старта:{projects[j].StartDate} дата окончания:{projects[j].EndDate}");
                }
                else
                {
                    Console.WriteLine(
                        $"Проект: {projects[j].Name} дата старта:{projects[j].StartDate} дата окончания: НЕ ЗАВЕРШЕН");
                }
            }
        }

        /// <summary>
        /// Выводит досье на сотрудника
        /// </summary>
        /// <param name="id"> Id струдника</param>
        static void DossieOnTheEmployee(int id)
        {
            var employee = _context.Employees.Where(e => e.EmployeeId == id).Select(e => e).ToList();

            var project = new List<Project>();
            foreach (var varEmployee in employee)
            {
                project = _context.EmployeesProjects.Where(e => e.EmployeeId == varEmployee.EmployeeId)
                    .Select(e => e.Project).ToList();
            }

            Console.WriteLine(
                $"{employee[0].LastName} {employee[0].FirstName} {employee[0].MiddleName} - {employee[0].JobTitle}");
            for (int i = 0; i < project.Count; i++)
            {
                if (project[i].EndDate != null)
                {
                    Console.WriteLine(
                        $"\n Project name: {project[i].Name} \n Project description: {project[i].Description} \n Start date:{project[i].StartDate} \n End date: {project[i].EndDate}");
                }
                else
                {
                    Console.WriteLine(
                        $"\n Project name: {project[i].Name} \n Project description: {project[i].Description} \n Start date:{project[i].StartDate} \n End date: Not complete");
                }

                var v = new StringBuilder();
                v.Append('-', 170);
                Console.Write(v);
            }
        }
        
        /// <summary>
        /// Функцияя ищет отделы с кол-ом сотрудинков < 5
        /// </summary>
        static void SmallDepartment()
        {
            var department = _context.Employees.GroupBy(e => e.DepartmentId).Select(e => new {Count = e.Count(), Key =e.Key}).ToList();
            foreach (var VARIABLE in department)
            {
                var dep = _context.Departments.SingleOrDefault(e => e.DepartmentId == VARIABLE.Key)?.Name;
                if (VARIABLE.Count<5)
                {
                    Console.WriteLine($"{dep} - {VARIABLE.Count}");
                }
            }
        }
        
        
        /// <summary>
        /// Функция увеличивает зарплаты в выбранном отделе
        /// </summary>
        /// <param name="dept">Название отдела</param>
        /// <param name="persent">процент увелечения</param>
        static void SalaryIncrease(string dept, int persent)
        {
             var emp = _context.Employees.Where(e => e.Department.Name.Equals(dept))
                .Select(e => e).ToList();
            foreach (var e in emp)
            {
                e.Salary += e.Salary * (decimal)(persent / 100.0);
                _context.SaveChanges();
               
            }
        }
        
        /// <summary>
        /// Функция удаляет отдел по id
        /// </summary>
        /// <param name="id">Id отдела</param>
        static void DeleteDepartment(int id)
        {
            var emp = _context.Employees.Where(e => e.Department.DepartmentId == id).Select(e => e).ToList();
            var dep = _context.Departments.FirstOrDefault(e => e.DepartmentId != id);
            foreach (var vEmployee in emp)
            {
                vEmployee.DepartmentId=dep.DepartmentId;
                _context.SaveChanges();
            }

            var delDep = _context.Departments.Where(e => e.DepartmentId == id).ToList();
            foreach (var variablDepartment in delDep)
            {
                var del = _context.Departments.Remove(variablDepartment);
                _context.SaveChanges();
            }
        }
        
        /// <summary>
        /// Функция удаляет город по названию
        /// </summary>
        /// <param name="name">Название города</param>
        static void DeleteTown(string name)
        {
            var address = _context.Addresses.Where(e => e.Town.Name.Equals(name)).Select(e=>e).ToList();
            foreach (var variablAddress in address)
            {
                variablAddress.TownId = null;
                _context.SaveChanges();
            }

            var town = _context.Towns.Where(e => e.Name.Equals(name)).ToList();
            foreach (var variablTown in town)
            {
                var del = _context.Towns.Remove(variablTown);
                _context.SaveChanges();
            }
        }
    }
}