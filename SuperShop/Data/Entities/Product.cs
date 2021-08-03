using System;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class Product : IEntity // Entidade que vai dar origem a uma tabela na base de dados
    {
        public int Id { get; set; } // como é Id automaticamente fica como chave primária

        [Required]
        [MaxLength(50,ErrorMessage ="The Field {0} can contain {1} characters length.")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] // c->currency Formato de moeda com duas casas decimais mas não no modo edição
        public decimal Price { get; set; }

        [Display(Name = "Image")] // Na página da web vai aparecer Image e não ImageUrl como nome do campo
        public string ImageUrl { get; set; } // Link da imagem dos produtos

        [Display(Name = "Last Purchase")] // Para aparecer com o espaço
        public DateTime? LastPurchase { get; set; } // última compra // ? -> o required field validator desaparece

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; } // última venda // ? -> o required field validator desaparece

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } // se o produto está disponível ou não

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)] // N-> number
        public double Stock { get; set; }
    }
}