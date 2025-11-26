namespace DataAccess.Models
{
    public class BookTag
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
        /// Тэг
        /// </summary>
        public Guid TagId { get; set; }

        /// <summary>
        /// Тэг (Навигационное свойство)
        /// </summary>
        public Tag? Tag { get; set; }
    }
}
