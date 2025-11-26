namespace Library.Pages.Book.Content
{
    public class ContentsModel
    {
        public DataAccess.Models.Content Content { get; private set; }

        public string Authors =>
            string.Join("; ", Content.Authors.Select(x => x.Name));

        public string Translators =>
            string.Join("; ", Content.Translators.Select(x => x.Name));

        public ContentsModel(DataAccess.Models.Content content) =>
            Content = content;
    }
}
