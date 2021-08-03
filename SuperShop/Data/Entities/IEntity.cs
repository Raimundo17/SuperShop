namespace SuperShop.Data.Entities
{
    public interface IEntity
    {
        int Id { get; set; } // comum a todas as entidades

        //bool WasDeleted { get; set; } // por defeito o bool é falso serve para "apagar registos", nao devo apagar
        //o utilizador "apaga" o registo mas na base de dados não é apagado
    }
}
