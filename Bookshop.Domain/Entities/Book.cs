namespace Bookshop.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string BookName { get; set; } = string.Empty;
        public decimal BookPrice { get; set; }
    }
}
