namespace P7CreateRestApi.Models
{
    public class RoleCollection
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly string[] Roles = { Admin, User };
    }
}
