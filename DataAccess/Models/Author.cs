using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Author : EntityBase
    {
        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; } = 
            string.Empty;

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateOnly? DateBirth { get; set; }

        /// <summary>
        /// Дата смерти
        /// </summary>
        public DateOnly? DateDeath { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; } = 
            string.Empty;

        /// <summary>
        /// Имя файла изображения
        /// </summary>
        /// <remarks>
        /// Данное свойство необходимо из-за ошибок в стратегии кэширования
        /// </remarks>
        public string ImageName { get; set; } =
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
