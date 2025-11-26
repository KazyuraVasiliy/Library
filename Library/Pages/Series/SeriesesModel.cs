namespace Library.Pages.Series
{
    public partial class SeriesesModel
    {
        public DataAccess.Models.Series Series { get; private set; }

        public SeriesesModel(DataAccess.Models.Series series) =>
            Series = series;
    }
}
