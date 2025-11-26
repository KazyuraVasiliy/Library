using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Tag : CollectionBase
    {
        /// <summary>
        /// Книги (Навигационное свойство)
        /// </summary>
        public virtual List<Book> Books { get; set; } =
            new();
    }
}
