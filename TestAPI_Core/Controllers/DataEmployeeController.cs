using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI_Core.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;
using TestAPI_Core.Services;

namespace TestAPI_Core.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataEmployeeController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly CounterService _counterService;
        private readonly ILogger<LoginController> _logger;

        public DataEmployeeController(IConfiguration configuration,CounterService counterService, ILogger<LoginController> logger)
        {
            _connectionString = configuration.GetConnectionString("Test_db");
            _counterService = counterService;
            _logger = logger;
        }
        [HttpGet] // Get all data
        public IActionResult Getdata()
        {
            var employees = new List<Employeesdata>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT stuid[ID], Employeename, EmployeeDept,Emppwd FROM student";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employeesdata
                                {
                                    Id = reader.GetInt32(0),
                                    Employeename = reader.GetString(1),
                                    EmployeeDept = reader.GetString(2),
                                    Employeepwd = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPost] // Put all data
        public IActionResult PutData([FromQuery] string Empname, string EmpDept, string Email, string Password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string maxid = "select max(stuid) from student";
                    SqlCommand cmd1 = new SqlCommand(maxid, connection);
                    connection.Open();
                    int max_id = int.Parse(cmd1.ExecuteScalar().ToString());
                    string query = "insert into student (stuid,name,employeename, employeedept,email,emppwd) values(@maxid,'',@employeename, @employeedept,@email,@password)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@maxid", max_id+1);
                    cmd.Parameters.AddWithValue("@employeename", Empname);
                    cmd.Parameters.AddWithValue("@employeedept", EmpDept);
                    cmd.Parameters.AddWithValue("@email", Email);
                    cmd.Parameters.AddWithValue("@password", Password);
                    int result = cmd.ExecuteNonQuery();
                }
                return Ok("Inserted Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteData([FromQuery]string Id)
        {
            try
            {
                using (SqlConnection sc = new SqlConnection(_connectionString))
                {
                    string query = "Delete from student where stuid = @id";
                    SqlCommand cmd = new SqlCommand(query, sc);
                    cmd.Parameters.AddWithValue("@id", Id);
                    sc.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return Ok("Deleted Successfully");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut]
        public IActionResult putdata([FromQuery]int Id ,string EmployeePassword, string Employee_Dept)
        {
            try
            {
                using (SqlConnection sc = new SqlConnection(_connectionString))
                {
                    string query = "update student set EmpPwd = @Emppwd,EmployeeDept = @Emp_dpt where stuid = @id";
                    SqlCommand cmd = new SqlCommand(query, sc);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.Parameters.AddWithValue("@Emppwd", EmployeePassword);
                    cmd.Parameters.AddWithValue("@Emp_dpt", Employee_Dept);
                    sc.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return Ok("Updated Successfully");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("counter")]    
        public IActionResult Getcount()
        {
            int count = _counterService.Increment();
            _logger.LogInformation("✅ Count Incremented to {count}", count);
            _logger.LogTrace("✅ This is a trace log.");
            _logger.LogDebug("✅ Debugging data.");
            _logger.LogInformation("✅ User logged in.");
            _logger.LogWarning("✅ Low disk space warning.");
            _logger.LogError("✅ Database connection failed!");
            _logger.LogCritical("✅ Critical system failure!");

            return Ok(new { count = count });
        }
    }
}
