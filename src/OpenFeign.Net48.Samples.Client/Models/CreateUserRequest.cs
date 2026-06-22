namespace OpenFeign.Net48.Samples.Client.Models
{
    public class CreateUserRequest
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? City { get; set; }

        public CreateUserRequest() { }
        public CreateUserRequest(string name, int age, string? city = null)
        {
            Name = name;
            Age = age;
            City = city;
        }
    }
}
