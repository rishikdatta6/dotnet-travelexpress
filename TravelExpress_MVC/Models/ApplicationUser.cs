using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Add custom properties like FirstName, LastName if needed
    public string Name { get; set; }
}
