namespace DataAccess.Models
{
    public class BookTranslator
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
        /// Переводчик
        /// </summary>
        public Guid TranslatorId { get; set; }

        /// <summary>
        /// Переводчик (Навигационное свойство)
        /// </summary>
        public Translator? Translator { get; set; }
    }
}
