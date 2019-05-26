using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace EmployeeApi.IntegrationTests
{
    public class EmployeeControllerTests : IClassFixture<EmployeeControllerTestFactory>
    {
        private readonly HttpClient _client;

        public EmployeeControllerTests(EmployeeControllerTestFactory factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task Get_All_Employees_Response_OK()
        {
            var response = await _client.GetAsync("/api/employee");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response);
            var responseString = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseString);
            Assert.True(employees.Any());
            Assert.True(employees.Any(x => x.City == "Chicago"));
        }

        [Fact]
        public async Task Get_employee_by_id_and_response_ok()
        {
            var response = await _client.GetAsync("/api/employee/1");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response);
            var responseString = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(responseString);
            Assert.True(employee.Id == 1);
        }

        [Fact]
        public async Task Post_create_new_employee_and_response_ok()
        {
            var newEmployee = new Employee()
            {
                Id = 99,
                City = "Austin",
                Name = "Sam"
            };
            var response = await _client.PostAsync("/api/employee",
                new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            Assert.True(response.StatusCode == HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_create_invalid_employee_and_response_bad_request()
        {
            var newEmployee = new Employee()
            {
                City = "SFO",
            };
            var response = await _client.PostAsync("/api/employee",
                new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json"));
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_modify_employee_and_response_ok()
        {
            var employeeResponse = await _client.GetAsync("/api/employee/1");
            var responseString = await employeeResponse.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(responseString);
            employee.Name = "Jon";
            var putResponse = await _client.PutAsJsonAsync<Employee>("/api/employee", employee);
            putResponse.EnsureSuccessStatusCode();
            var putResponseString = await putResponse.Content.ReadAsStringAsync();
            var updatedEmployee = JsonConvert.DeserializeObject<Employee>(putResponseString);
            Assert.True(updatedEmployee.Name.Equals("Jon"));
        }

        [Fact]
        public async Task Delete_employee_and_response_no_content()
        {
            var response = await _client.DeleteAsync($"api/employee/3");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);
        }
    }
}