using Microsoft.AspNetCore.Identity;

internal class ApplicationUser : IdentityRole
{
    public ApplicationUser(string roleName) : base(roleName)
    {
    }
}