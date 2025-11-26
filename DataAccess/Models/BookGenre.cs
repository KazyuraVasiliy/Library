namespace DataAccess.Models
{
    public class BookGenre
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
        /// Жанр
        /// </summary>
        public Guid GenreId { get; set; }

        /// <summary>
        /// Жанр (Навигационное свойство)
        /// </summary>
        public Genre? Genre { get; set; }
    }
}
