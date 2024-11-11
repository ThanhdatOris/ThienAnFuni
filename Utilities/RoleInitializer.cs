using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class RoleInitializer
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Manager", "SaleStaff", "Customer" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }
    }
}
