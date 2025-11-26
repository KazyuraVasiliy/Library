using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Language : CollectionBase
    {
        /// <summary>
        /// Книги (Навигационное свойство)
        /// </summary>
        public virtual List<Book> Books { get; set; } =
            new();

        /// <summary>
        /// Содержание книги (список произведений)
        /// </summary>
        public virtual List<Content> Contents { get; set; } =
            new();
    }
}
