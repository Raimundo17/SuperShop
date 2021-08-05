using SuperShop.Data.Entities;
using SuperShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
   public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        ProductViewModel ToProductViewModel(Product product);
    }
}
