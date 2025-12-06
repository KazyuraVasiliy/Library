namespace Library.Services
{
    public static class PreferencesService
    {
        public static class Book
        {
            /// <summary>
            /// Сортировка
            /// </summary>
            public static int SortField
            {
                get => Preferences.Default.Get("book_sort_field", 4);
                set => Preferences.Default.Set("book_sort_field", value);
            }

            /// <summary>
            /// Режим отображения
            /// </summary>
            public static int ViewMode
            {
                get => Preferences.Default.Get("book_view_mode", 1);
                set => Preferences.Default.Set("book_view_mode", value);
            }
        }
    }
}
