namespace DataAccess.Models.Import
{
    public class Author
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
    }
}
