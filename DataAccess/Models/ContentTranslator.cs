namespace DataAccess.Models
{
    public class ContentTranslator
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
        /// Переводчик
        /// </summary>
        public Guid TranslatorId { get; set; }

        /// <summary>
        /// Переводчик (Навигационное свойство)
        /// </summary>
        public Translator? Translator { get; set; }
    }
}
