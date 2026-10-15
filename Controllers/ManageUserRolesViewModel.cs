namespace RunWay.Models.ViewModels
{
    public class ManageUserRolesViewModel // <-- must be public
    {
        internal bool isSelected;

        public string RoleId { get; set; } = string.Empty; // fixes CS8618
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; } // use PascalCase
    }

    public class UserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}