namespace Library
{
    public partial class App : Application
    {
        private readonly AppShellViewModel _appShellViewModel;

        public App(AppShellViewModel viewModel)
        {
            InitializeComponent();
            _appShellViewModel = viewModel;
        }

        protected override Window CreateWindow(IActivationState? activationState) =>
            new Window(new AppShell()
            {
                BindingContext = _appShellViewModel
            });
    }
}
