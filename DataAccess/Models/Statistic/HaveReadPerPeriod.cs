namespace DataAccess.Models.Statistic
{
    public enum PerioTypes
    {
        Month,
        Year
    }

    public class HaveReadPerPeriod
    {
        /// <summary>
        /// Тип периода
        /// </summary>
        public PerioTypes PeriodType;

        /// <summary>
        /// Период (определённый месяц или год)
        /// </summary>
        public DateOnly Period { get; set; }

        /// <summary>
        /// Кол-во прочитанных книг за период
        /// </summary>
        public int Count { get; set; }
    }
}
