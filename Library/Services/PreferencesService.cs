namespace Library.Services
{
    public static class PreferencesService
    {
        public static class Book
        {
            public static int SortField
            {
                get => Preferences.Default.Get("book_sort_field", 4);
                set => Preferences.Default.Set("book_sort_field", value);
            }
        }
    }
}
