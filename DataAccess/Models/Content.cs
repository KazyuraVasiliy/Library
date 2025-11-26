using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Content : EntityBase
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = 
            string.Empty;

        /// <summary>
        /// Авторы
        /// </summary>
        public virtual List<Author> Authors { get; set; } =
            new();

        /// <summary>
        /// Авторы (Связка)
        /// </summary>
        public virtual List<ContentAuthor> ContentAuthors { get; set; } =
            new();

        /// <summary>
        /// Переводчики
        /// </summary>
        public virtual List<Translator> Translators { get; set; } =
            new();

        /// <summary>
        /// Переводчики (Связка)
        /// </summary>
        public virtual List<ContentTranslator> ContentTranslators { get; set; } =
            new();

        /// <summary>
        /// Год написания
        /// </summary>
        public short? WriteYear { get; set; }

        /// <summary>
        /// Язык
        /// </summary>
        public Guid? LanguageId { get; set; }

        /// <summary>
        /// Язык (Навигационное свойство)
        /// </summary>
        public virtual Language? Language { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public Guid? StatusId { get; set; } =
            Constants.Statuses.HaveNotRead;

        /// <summary>
        /// Статус (Навигационное свойство)
        /// </summary>
        public virtual Status? Status { get; set; }

        /// <summary>
        /// Серия
        /// </summary>
        public Guid? SeriesId { get; set; }

        /// <summary>
        /// Серия (Навигационное свойство)
        /// </summary>
        public virtual Series? Series { get; set; }

        /// <summary>
        /// Номер в серии
        /// </summary>
        public double? SeriesNumber { get; set; }

        /// <summary>
        /// Страница
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Уровень в содержании
        /// </summary>
        public double? Level { get; set; }

        /// <summary>
        /// Книга
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Книга (Навигационное свойство)
        /// </summary>
        public virtual Book? Book { get; set; }
    }
}
