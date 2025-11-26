using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Series : EntityBase
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = 
            string.Empty;

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; } =
            string.Empty;

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
