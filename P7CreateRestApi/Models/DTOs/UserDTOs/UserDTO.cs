namespace P7CreateRestApi.Models.DTOs.UserDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
    }
}
