using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace Library.Services.ProgressService
{
    public class ProgressService
    {
        private Page? mainPage;

        public virtual bool StartProgress(object bindingContext)
        {
            var popup = new ProgressView();
            popup.BindingContext = bindingContext;

            var popupOptions = new PopupOptions()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };

            if (mainPage == null)
                mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (mainPage == null)
                throw new Exception("Отсутствует основная страница");

            mainPage.ShowPopup(popup, popupOptions);
            return true;
        }

        public virtual async Task CloseProgress(CancellationToken cancellationToken = default)
        {
            if (mainPage != null)
                await mainPage.ClosePopupAsync(cancellationToken);
        }
    }
}
