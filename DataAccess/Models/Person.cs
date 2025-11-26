using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Person : EntityBase
    {
        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; } =
            string.Empty;

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
        /// Книги в собственности у субъекта (Навигационное свойство)
        /// </summary>
        public virtual List<Book> MyBooks { get; set; } =
            new();

        /// <summary>
        /// Книги, находящиеся у субъекта в текущий момент (Навигационное свойство)
        /// </summary>
        public virtual List<Book> LocationBooks { get; set; } =
            new();
    }
}
