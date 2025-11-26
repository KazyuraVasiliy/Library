using Microsoft.Maui.Graphics.Platform;

namespace Library.Services
{
    public class ImageService
    {
        private readonly PermissionService _permissionService;

        public ImageService(PermissionService permissionService) =>
            _permissionService = permissionService;

        private void RecreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
            Directory.CreateDirectory(directoryPath);
        }

        public virtual async Task<FileResult?> GetTempImageFromStorageAsync(string destinationFilePath)
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions()
            {
                FileTypes = FilePickerFileType.Jpeg
            });

            if (file != null)
            {
                RecreateDirectory(Path.GetDirectoryName(destinationFilePath)!);
                File.Copy(file.FullPath, destinationFilePath);
            }

            return file;
        }

        public virtual async Task GetTempImageFromUriAsync(string uri, string destinationFilePath)
        {
            using var client = new HttpClient();
            using var stream = await client.GetStreamAsync(uri);

            RecreateDirectory(Path.GetDirectoryName(destinationFilePath)!);

            using var fileStream = new FileStream(destinationFilePath, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);
        }

        public virtual void GetTempImageFromFileAsync(string sourceFilePath, string tempFilePath)
        {
            var tempDirectoryPath = Path.GetDirectoryName(tempFilePath);

            RecreateDirectory(tempDirectoryPath!);
            File.Copy(sourceFilePath, tempFilePath);
        }

        public virtual Task RemoveImage(string destinationFilePath)
        {
            File.Delete(destinationFilePath);
            return Task.FromResult(true);
        }

        public virtual Task CopyImage(string sourceFilePath, string destinationFilePath)
        {
            File.Copy(sourceFilePath, destinationFilePath);
            return Task.FromResult(true);
        }

        private static (int width, int height) ResizeDimensions(float originalWidth, float originalHeight, int maxWidth, int maxHeight)
        {
            float ratioX = maxWidth / originalWidth;
            float ratioY = maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            return (newWidth, newHeight);
        }

        public virtual async Task CreateThumbImageAsync(string sourceFilePath, string thumbFilePath, int maxWidth = 100, int maxHeight = 100)
        {
            if (!File.Exists(sourceFilePath))
                throw new Exception("Исходный файл не найден");

            await using var sourceFileStream = File.OpenRead(sourceFilePath);
            var image = PlatformImage.FromStream(sourceFileStream);

            var originalWidth = image.Width;
            var originalHeight = image.Height;

            var (newWidth, newHeight) = ResizeDimensions(originalWidth, originalHeight, maxWidth, maxHeight);
            using var resizedImage = image.Resize(newWidth, newHeight, ResizeMode.Fit);

            Directory.CreateDirectory(Path.GetDirectoryName(thumbFilePath)!);

            await using var thumbFileStream = File.Create(thumbFilePath);
            await resizedImage.SaveAsync(thumbFileStream, ImageFormat.Jpeg);
        }
    }
}
