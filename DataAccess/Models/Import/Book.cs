namespace DataAccess.Models.Import
{
    public class Book
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; } =
            Guid.NewGuid();

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } =
            DateTime.Now;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = 
            string.Empty;

        /// <summary>
        /// Авторы
        /// </summary>
        public List<Guid> Authors { get; set; } =
            new();

        /// <summary>
        /// Переводчики
        /// </summary>
        public List<Guid> Translators { get; set; } =
            new();

        /// <summary>
        /// Тэги
        /// </summary>
        public List<Guid> Tags { get; set; } =
            new();

        /// <summary>
        /// Жанры
        /// </summary>
        public List<Guid> Genres { get; set; } =
            new();

        /// <summary>
        /// Издатель
        /// </summary>
        public Guid? PublisherId { get; set; }

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
        /// Дата прочтения
        /// </summary>
        public DateOnly? DateRead { get; set; }

        /// <summary>
        /// Серия
        /// </summary>
        public Guid? SeriesId { get; set; }

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
        /// Местоположение
        /// </summary>
        public Guid? LocationId { get; set; }

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
        public List<Content> Contents { get; set; } =
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
