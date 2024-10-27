namespace ThienAnFuni.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Password { get; set; }

        public void ChangePassword() { }
        public void ForgotPassword() { }
        public void UpdateProfile() { }
    }
}
