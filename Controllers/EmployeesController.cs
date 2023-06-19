using EntityFrameworkPractical.Models;
using EntityFrameworkPractical.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using static Azure.Core.HttpHeader;
using System.Xml.Linq;
using System.Data.Entity.Migrations.Model;

namespace EntityFrameworkPractical.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MVCDemoDbContext mvcDemoDbContext;
        public EmployeesController(MVCDemoDbContext mvcDemoDbContext)
        {
            this.mvcDemoDbContext = mvcDemoDbContext;
        }

        //public MVCDemoDbContext MvcDemoDbContext { get; }

      // below indox method is working

        //[HttpGet]
        //public IActionResult Index()
        //{
        //   var employees= mvcDemoDbContext.Employees.ToList();
        //    return View(employees);
        //}
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddEmployeeViweModel addEmployeeRequest)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DOB=addEmployeeRequest.DOB
            };
             mvcDemoDbContext.Employees.Add(employee);
             mvcDemoDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult View1(Guid id)
        {
           var employee= mvcDemoDbContext.Employees.FirstOrDefault(x => x.Id == id);
            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DOB = employee.DOB
                };

                return View(nameof(View1),viewModel);
            }
           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult View1(UpdateEmployeeViewModel model)
        {
            var employee = mvcDemoDbContext.Employees.Find(model.Id);
            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Salary = model.Salary;
                employee.DOB = model.DOB;
                employee.Department = model.Department;


                mvcDemoDbContext.Employees.Update(employee);// this line I've added
                mvcDemoDbContext.SaveChanges();
                return RedirectToAction("Index");


            }
            return RedirectToAction("Index");

        }

        public IActionResult Delete(Guid id)
        {
            var employee = mvcDemoDbContext.Employees.Find(id);

            if (employee != null)
            {
                mvcDemoDbContext.Employees.Remove(employee);
                mvcDemoDbContext.SaveChanges();
                
            }
            return RedirectToAction("Index");
        }
        // How to call a SP from Controller Method


        public ActionResult Index(int isPaging, int min=0,int max=100, int Page_no = 0, int Page_Size = 10, string Emp_Name = " ", string[] CountryIds = null,string[] DepartmentIds=null, string Chronology="", string flag = "",string Prev_Column="", string IdValue="Name")
        {
            EmployeeViewModel employeesViewModel = new EmployeeViewModel();
            // first way to access view model property
            //{
            //    Departments = //Procedure2,
            //    employees = //Procedure 
            //};
            // second way to access viewModel data
            //employeesViewModel.employees = //Procedure ;
            //employeesViewModel.Departments = //Procedure2 ;


            string countryCsvQuery = "";
            if (CountryIds.Length<0 )
            {
                for (int i = 0; i < CountryIds.Length; i++)
                {
                    countryCsvQuery += CountryIds[i] + ",";
                }
            }

            string departmentCsvQuery = "";
            if (DepartmentIds.Length<0)
            {
                for (int i = 0; i < DepartmentIds.Length; i++)
                {
                    departmentCsvQuery += DepartmentIds[i] + ",";
                }
            }
            if (!string.IsNullOrEmpty(flag))
            {
                if (Prev_Column != null && Prev_Column != flag && isPaging != 1)
                {
                    Chronology = "0";
                }
            }
            
            if (isPaging != 1)
            {
                Chronology = Chronology == "0" ? "1" : "0";
            }
            isPaging = 0;

            string Uparrow = flag + "up";
            string Downarrow = flag + "down";

            ViewData["Idup"] = "none";
            ViewData["Nameup"] = "none";
            ViewData["Emailup"] = "none";
            ViewData["Salaryup"] = "none";
            ViewData["DOBup"] = "none";
            ViewData["Departmentup"] = "none";
            ViewData["Countryup"] = "none";


            ViewData["Iddown"] = "none";
            ViewData["Namedown"] = "none";
            ViewData["Emaildown"] = "none";
            ViewData["Salarydown"] = "none";
            ViewData["DOBdown"] = "none";
            ViewData["Departmentdown"] = "none";
            ViewData["Countrydown"] = "none";

            if (flag == "")
            {
                ViewData["Nameup"] = "inline";
            }

            if (Chronology=="1")
            {                
               // ViewData["Nameup"] = "none";
                ViewData[Downarrow] = "inline";
            }
            else
            {
                //ViewData["Nameup"] = "none";
                ViewData[Uparrow] = "inline";
            }
            if(Emp_Name is null) { Emp_Name = ""; }
            //var employees = mvcDemoDbContext.Employees.FromSqlRaw("EXEC ProcDemo @Page_no", new SqlParameter("@Page_no", Page_no)).ToList(); // working fine
            var param = new SqlParameter("@Page_no", Page_no);
            var param3 = new SqlParameter("@PageSize", Page_Size);
            var param4 = new SqlParameter("@Emp_Name", Emp_Name);
            var param7 = new SqlParameter("@Emp_Email", Emp_Name);
            var param5 = new SqlParameter("@Chronology", Chronology);
            var param8 = new SqlParameter("@flag", flag);
            var param6 = new SqlParameter
            {
                ParameterName = "@filter_count",
                DbType = DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            };

            var param9 = new SqlParameter("@countryCsvQuery", countryCsvQuery);
            var param10 = new SqlParameter("@departmentCsvQuery", departmentCsvQuery);
            var param11 = new SqlParameter("@minAge",min);
            var param12 = new SqlParameter("@maxAge", max);
            employeesViewModel.employees = mvcDemoDbContext.Employees.FromSqlRaw("EXEC ProcDemo23 @Page_no ,@PageSize,@Emp_Name,@Emp_Email,@Chronology,@flag ,@countryCsvQuery,@departmentCsvQuery,@minAge,@maxAge,@filter_count OUTPUT"
                , param, param3,param4, param7,param5,param8, param9,param10,param11,param12, param6).ToList();

            ViewData["Pg_size"] = Page_Size;// bcz page size is required in viwe while navigate through page index
            ViewData["Ename"] = Emp_Name;//bcz name is required in view
            ViewBag.page_no = Page_no;//bcz name is required in view
            ViewData["flag"] = flag;
            ViewData["Chronooo"] = Chronology;
            ViewData["prev_Column"] = flag;
            ViewBag.minAge = min;
            ViewBag.maxAge = max == 0 ? 100 : max;
            ViewBag.CountryIds = countryCsvQuery;
            ViewBag.DepartmentIds = departmentCsvQuery;

            // GET all unique department using Entityframework

            employeesViewModel.employees2 = mvcDemoDbContext.Employees.ToList();

            //Get all unique departments using SP
            //try
            //{
            //    var dept = mvcDemoDbContext.Employees.FromSqlRaw("EXEC GetDepartment").ToList();
            //    //employeesViewModel.Departments = mvcDemoDbContext.Employees.FromSqlRaw("EXEC GetDepartment ").ToList();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            //var paramDept = new SqlParameter
            //{
            //    ParameterName = "@deptcount",
            //    DbType = DbType.Int32,
            //    Direction = System.Data.ParameterDirection.Output
            //};
            //var Dept = mvcDemoDbContext.Employees.FromSqlRaw("EXEC GetDepartment").ToList();
            //employeesViewModel.Departments = Dept.SelectMany(x => x.Department);
            // end Get all unique departments

            var param2 = new SqlParameter
            {
                ParameterName = "@count",
                DbType = DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            };

            int totalEmployees;
            if ((string.IsNullOrWhiteSpace(Emp_Name)) && (CountryIds is null) && (DepartmentIds is null) )
            {
                mvcDemoDbContext.Database.ExecuteSqlRaw("EXEC getCount @count OUTPUT", param2);
                totalEmployees = (int)param2.Value;
            }
            else
            {
                //var Filetered_Emp = new SqlParameter("@totalEmployees", SqlDbType.Int)
                //{
                //    Direction = ParameterDirection.Output
                //};
                //employees = mvcDemoDbContext.Employees.FromSqlRaw("EXEC ProcDemo @Page_no ,@PageSize,@Emp_Name", param, param3, param4).ToList();
                totalEmployees = (int)param6.Value;

              //  totalEmployees = employees.Count();
            }
            

            if (totalEmployees % Page_Size == 0)
            {
                ViewBag.TotalPages = totalEmployees / Page_Size;
            }
            else
            {
                ViewBag.TotalPages = (totalEmployees / Page_Size) + 1;
            }
            return View(employeesViewModel);

        }

        // calling simple stored procedure

        //public ActionResult Index(int Page_no)
        //{
        //    // Call the stored procedure using FromSql method


        //    //var employees = mvcDemoDbContext.Employees.FromSqlRaw("EXEC ProcDemo @Page_no", new SqlParameter("@Page_no", Page_no)).ToList(); // working fine
        //    var param = new SqlParameter("@Page_no", Page_no);
        //    var employees = mvcDemoDbContext.Employees.FromSqlRaw("EXEC get_offset_fetch @Page_no", param).ToList();
        //}


    }
}
