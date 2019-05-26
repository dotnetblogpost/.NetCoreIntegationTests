using System.ComponentModel.DataAnnotations;

namespace EmployeeApi
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
[Required]
        public string Name { get; set; }

        public string City { get; set; }

    }
}
