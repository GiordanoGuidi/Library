namespace Library.Models.DTO
{
    public class RegisterUserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; } = null!;

        public string Email { get; set; }
        public string Password { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
    }
}
