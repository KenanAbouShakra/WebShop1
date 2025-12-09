using Microsoft.AspNetCore.Identity;
namespace WebShop1.Models
{
    public class AppUser : IdentityUser 
    {
        public ICollection<Order> Orders {  get; set; }
    }
}
