using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EF_Core_P1_P2.Data.Models;


namespace EFCore_LINQ_SQL
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
            SalaryIncrease("Executive", 10);
        }
        
        /// <summary>
        /// Функция вывода на экран всех сотрудников
        /// </summary>
        /// <returns></returns>
        static string GetEmployeesInformation()
        {
            
            var employees = from e in _context.Employees orderby (e.EmployeeId) select e;
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
            var employees = from e in _context.Employees orderby e.LastName where e.Salary > 48000 select e;
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
            var _address = new Address("", 1);
            var address = _context.Addresses.Add(_address);
            _context.SaveChanges();
            
            var employees = (from e in _context.Employees where e.LastName.Equals("Brown") select e).ToList();
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
            
            var projects = (from p in _context.Projects
                where (p.StartDate.Year <= 2005 && p.StartDate.Year >= 2002)
                select p).ToList();
            var employees = _context.Employees.ToList();
            for (int i = 0; i < projects.Count; i++)
            {
                var employeesProjects =
                    (from e in _context.EmployeesProjects where e.ProjectId == projects[i].ProjectId select e.Employee).Take(5)
                    .ToList();
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
            var employee = (from e in _context.Employees where e.EmployeeId == id select e).ToList();
            var project = new List<Project>();
            foreach (var varEmployee in employee)
            {
                project = (from e in _context.EmployeesProjects
                    where e.EmployeeId == varEmployee.EmployeeId
                    select e.Project).ToList();
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
            var department = (from e in _context.Employees
                group e by e.DepartmentId
                into d
                select new {Count = d.Count(), Key = d.Key}).ToList();
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
            var emp = (from e in _context.Employees where e.Department.Name.Equals(dept) select e).ToList();
            
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
           
            var emp = (from e in _context.Employees where e.Department.DepartmentId == id select e).ToList();
            var dep = (from e in _context.Departments where e.DepartmentId!=id 
                select e).FirstOrDefault();
            foreach (var vEmployee in emp)
            {
                vEmployee.DepartmentId=dep.DepartmentId;
                _context.SaveChanges();
            }
            
            var delDep = (from e in _context.Departments where e.DepartmentId == id select e).ToList();
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
            
            var address = (from e in _context.Addresses where e.Town.Name.Equals(name) select e).ToList();
            foreach (var variablAddress in address)
            {
                variablAddress.TownId = null;
                _context.SaveChanges();
            }
            
            var town = (from e in _context.Towns where e.Name.Equals(name) select e).ToList();
            foreach (var variablTown in town)
            {
                var del = _context.Towns.Remove(variablTown);
                _context.SaveChanges();
            }
        }
        
        
        
        
        
    }
}