namespace DataAccess.Models
{
    public class BookAuthor
    {
        /// <summary>
        /// Книга
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Книга (Навигационное свойство)
        /// </summary>
        public Book? Book { get; set; }

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
