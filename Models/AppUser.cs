using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sistema_Cinema.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres")]
        public string Nome { get; set; }

        public Cliente Cliente { get; set; }
    }
}
