namespace DataAccess.Models.Import
{
    public class Series
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
        /// Комментарий
        /// </summary>
        public string Comment { get; set; } =
            string.Empty;
    }
}
