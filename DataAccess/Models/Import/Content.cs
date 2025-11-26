namespace DataAccess.Models.Import
{
    public class Content 
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
        /// Год написания
        /// </summary>
        public short? WriteYear { get; set; }

        /// <summary>
        /// Язык
        /// </summary>
        public Guid? LanguageId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public Guid? StatusId { get; set; } =
            Constants.Statuses.HaveNotRead;

        /// <summary>
        /// Серия
        /// </summary>
        public Guid? SeriesId { get; set; }

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
    }
}
