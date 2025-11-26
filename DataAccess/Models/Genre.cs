using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Genre : CollectionBase
    {
        /// <summary>
        /// Книги (Навигационное свойство)
        /// </summary>
        public virtual List<Book> Books { get; set; } =
            new();
    }
}
