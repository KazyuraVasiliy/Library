using DataAccess.Models.Base;

namespace DataAccess.Models
{
    public class Book : EntityBase
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
        public virtual List<BookAuthor> BookAuthors { get; set; } =
            new();

        /// <summary>
        /// Переводчики
        /// </summary>
        public virtual List<Translator> Translators { get; set; } =
            new();

        /// <summary>
        /// Переводчики (Связка)
        /// </summary>
        public virtual List<BookTranslator> BookTranslators { get; set; } =
            new();

        /// <summary>
        /// Тэги
        /// </summary>
        public virtual List<Tag> Tags { get; set; } =
            new();

        /// <summary>
        /// Тэги (Связка)
        /// </summary>
        public virtual List<BookTag> BookTags { get; set; } =
            new();

        /// <summary>
        /// Жанры
        /// </summary>
        public virtual List<Genre> Genres { get; set; } =
            new();

        /// <summary>
        /// Жанры (Связка)
        /// </summary>
        public virtual List<BookGenre> BookGenres { get; set; } =
            new();

        /// <summary>
        /// Издатель
        /// </summary>
        public Guid? PublisherId { get; set; }

        /// <summary>
        /// Издатель (Навигационное свойство)
        /// </summary>
        public virtual Publisher? Publisher { get; set; }

        /// <summary>
        /// Год издания
        /// </summary>
        public short? PublishYear { get; set; }

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
        /// Бумажная книга
        /// </summary>
        public bool IsPaperBook { get; set; }

        /// <summary>
        /// Электронная книга
        /// </summary>
        public bool IsEBook { get; set; }

        /// <summary>
        /// Аудио книга
        /// </summary>
        public bool IsAudioBook { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public Guid StatusId { get; set; } =
            Constants.Statuses.HaveNotRead;

        /// <summary>
        /// Статус (Навигационное свойство)
        /// </summary>
        public virtual Status? Status { get; set; }

        /// <summary>
        /// Дата прочтения
        /// </summary>
        public DateOnly? DateRead { get; set; }

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
        /// ISBN (Международный стандартный книжный номер)
        /// </summary>
        public string ISBN { get; set; } =
            string.Empty;

        /// <summary>
        /// Комплексный книготорговый индекс-шифр
        /// </summary>
        public string IndexCode { get; set; } =
            string.Empty;

        /// <summary>
        /// УДК (Универсальная десятичная классификация)
        /// </summary>
        public string UDC { get; set; } =
            string.Empty;

        /// <summary>
        /// ББК (Библиотечно-библиографическая классификация)
        /// </summary>
        public string LBC { get; set; } =
            string.Empty;

        /// <summary>
        /// Владелец
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Владелец (Навигационное свойство)
        /// </summary>
        public virtual Person? Owner { get; set; }

        /// <summary>
        /// Местоположение
        /// </summary>
        public Guid? LocationId { get; set; }

        /// <summary>
        /// Местоположение (Навигационное свойство)
        /// </summary>
        public virtual Person? Location { get; set; }

        /// <summary>
        /// Аннотация
        /// </summary>
        public string Annotation { get; set; } = 
            string.Empty;

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; } = 
            string.Empty;

        /// <summary>
        /// Содержание книги (список произведений)
        /// </summary>
        public virtual List<Content> Contents { get; set; } =
            new();

        /// <summary>
        /// Имя файла изображения
        /// </summary>
        /// <remarks>
        /// Данное свойство необходимо из-за ошибок в стратегии кэширования
        /// </remarks>
        public string ImageName { get; set; } = 
            string.Empty;
    }
}
