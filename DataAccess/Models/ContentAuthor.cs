namespace DataAccess.Models
{
    public class ContentAuthor
    {
        /// <summary>
        /// Содержание
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Содержание (Навигационное свойство)
        /// </summary>
        public Content? Content { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Автор (Навигационное свойство)
        /// </summary>
        public Author? Author { get; set; }
    }
}
