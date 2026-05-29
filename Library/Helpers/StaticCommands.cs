namespace Library.Helpers
{
    public static class StaticCommands
    {
        /// <summary>
        /// Сбрасывает выбранный элемент
        /// </summary>
        public static Command ResetCommand =>
            new Command((obj) =>
            {
                if (obj is Picker picker)
                    picker.SelectedIndex = -1;
            });
    }
}
