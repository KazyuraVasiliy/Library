namespace DataAccess.Models.Base
{
    public class EntityBase
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

        public override bool Equals(object? obj) =>
            obj == null || GetType() != obj.GetType()
                ? false
                : Id == (obj as EntityBase)?.Id;

        public override int GetHashCode() => 
            Id.GetHashCode();
    }
}
