namespace DataAccess
{
    public static class Constants
    {
        public static class Statuses
        {
            /// <summary>
            /// Не читал
            /// </summary>
            public static readonly Guid HaveNotRead =
                Guid.Parse("76105A84-83C4-4888-A060-F3393406E4BC");

            /// <summary>
            /// Хочу прочитать
            /// </summary>
            public static readonly Guid WantRead =
                Guid.Parse("A85190B6-8C50-4268-8BBA-FDB713DA15CA");

            /// <summary>
            /// Читаю сейчас
            /// </summary>
            public static readonly Guid Reading =
                Guid.Parse("AFEEE47A-E705-4FAD-B0A4-B60BBA605839");

            /// <summary>
            /// Прочёл
            /// </summary>
            public static readonly Guid HaveRead =
                Guid.Parse("82437C70-47EE-46A4-81BD-6CF26DEF19A2");
        }
    }
}
