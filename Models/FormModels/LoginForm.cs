namespace ContestSystem.Models.FormModels
{
    public class LoginForm
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
        public string Fingerprint { get; set; }
    }
}