using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkPractical.Models.Domain
{

    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Salary { get; set; }
        public DateTime DOB { get; set;}
        public string Department { get; set; }
        public string Country { get; set; }

    }

}
