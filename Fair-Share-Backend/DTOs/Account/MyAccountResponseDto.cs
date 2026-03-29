namespace Fair_Share_Backend.DTOs.Account
{
    public class MyAccountResponseDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Points { get; set; }
        public int TasksAssigned { get; set; }
    }
}