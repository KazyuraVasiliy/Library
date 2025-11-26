using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Models.Import;
using DataAccess.Services;
using Library.Services;
using Library.Services.ProgressService;
using System.IO.Compression;
using System.Text.Json;

namespace Library
{   
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly string _appDataDirectory;
        private readonly ImportService _importService;
        private readonly ImageService _imageService;
        private readonly ProgressService _progressService;

        public string Version =>
            AppInfo.VersionString;

        [ObservableProperty]
        private string _progressDescription =
            string.Empty;

        public AppShellViewModel(string appDataDirectory, ImportService importService, ImageService imageService, ProgressService progressService)
        {
            _appDataDirectory = appDataDirectory;
            _importService = importService;
            _imageService = imageService;
            _progressService = progressService;
        }

        [RelayCommand]
        private async Task Import()
        {
            bool isStartProgress = false;

            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/zip" } },
                    { DevicePlatform.WinUI, new[] { ".zip" } }
                });

            try
            {
                var file = await FilePicker.Default.PickAsync(new PickOptions()
                {
                    FileTypes = customFileType,
                    PickerTitle = "Выберите zip архив"
                });

                if (file != null && Path.GetExtension(file.FileName) == ".zip")
                {
                    isStartProgress = _progressService.StartProgress(this);
                    ProgressDescription = "Проверка архива";

                    var zipArchive = ZipFile.OpenRead(file.FullPath);
                    var checkDataFile = zipArchive.Entries.Any(x => x.Name == "library.json" || x.Name == "library.db");

                    if (!checkDataFile)
                        throw new Exception("В архиве не найден файл данных");

                    ProgressDescription = "Распаковка";
                    zipArchive.ExtractToDirectory(_appDataDirectory, true);

                    var jsonPath = Path.Combine(_appDataDirectory, "library.json");
                    if (File.Exists(jsonPath))
                    {
                        var json = File.ReadAllText(jsonPath);
                        var importObject = JsonSerializer.Deserialize<General>(json);

                        if (importObject != null)
                        {
                            ProgressDescription = "Сохранение данных";
                            await _importService.ImportDataAsync(importObject);

                            string[] imageDirectories = [
                                Path.Combine(_appDataDirectory, "books", "images"),
                                Path.Combine(_appDataDirectory, "authors", "images"),
                                Path.Combine(_appDataDirectory, "persons", "images"),
                            ];

                            int directoryIterator = 1;

                            foreach (var imageDirectory in imageDirectories)
                            {
                                var images = Directory.GetFiles(imageDirectory);
                                var thumbs = Directory.Exists(Path.Combine(imageDirectory, "thumbs"))
                                    ? Directory.GetFiles(Path.Combine(imageDirectory, "thumbs")).ToDictionary(x => x)
                                    : new Dictionary<string, string>();

                                int imageIterator = 1;

                                foreach (var image in images)
                                {
                                    ProgressDescription = $"Обработка изображений {directoryIterator}/{imageDirectories.Length} : {imageIterator++}/{images.Length}";

                                    var thumbPath = Path.Combine(Path.GetDirectoryName(image)!, "thumbs", Path.GetFileName(image));
                                    if (!thumbs.ContainsKey(thumbPath))
                                        await _imageService.CreateThumbImageAsync(image, thumbPath);
                                }

                                directoryIterator++;
                            }
                        }

                        File.Delete(jsonPath);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
            finally
            {
                if (isStartProgress)
                    await _progressService.CloseProgress();
            }
        }
    }
}
