using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    public class LoginViewModel // Modelo responsável pelo login ; Como não tem nada a haver com a tabela não é uma
                                // entidade. É uma view Model
    {
        [Required]
        [EmailAddress] // só aceita emails (deteta a @)
        public string UserName { get; set; } // Vai corresponder ao email

        [Required]
        [MinLength(6)] //só funciona para a view (para dar as mensagens ao utilizador)
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
