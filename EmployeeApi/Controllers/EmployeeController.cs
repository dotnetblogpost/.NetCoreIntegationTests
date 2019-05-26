using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _dbContext;

        public EmployeeController(EmployeeDbContext employeeDbContext)
        {
            _dbContext = employeeDbContext;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> Get()
        {
            return _dbContext.Employees.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            return _dbContext.Employees.SingleOrDefault(x => x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] Employee newEmployee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Employees.Add(newEmployee);
                _dbContext.SaveChanges();
                return Created($"/api/employee/{newEmployee.Id}", newEmployee);
            }
            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut]
        public ActionResult Put(int id, [FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Employees.Update(employee);
                _dbContext.SaveChanges();
                return Ok(employee);
            }
            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var employeee =  _dbContext.Employees.Find(id);
            _dbContext.Employees.Remove(employeee);
            return NoContent();
        }
    }
}