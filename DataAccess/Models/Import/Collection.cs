namespace DataAccess.Models.Import
{
    public class Collection
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; } =
            Guid.NewGuid();

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } =
            string.Empty;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } =
            DateTime.Now;
    }
}
