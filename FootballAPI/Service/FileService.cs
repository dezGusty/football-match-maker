using FootballAPI.Service.Interfaces;

namespace FootballAPI.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string _profileImagesPath;
        private readonly string _allowedExtensions = ".jpg,.jpeg,.png,.gif,.webp";
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
            _profileImagesPath = Path.Combine(_environment.WebRootPath, "uploads", "profile-images");

            if (!Directory.Exists(_profileImagesPath))
            {
                Directory.CreateDirectory(_profileImagesPath);
            }
        }

        public async Task<string> SaveProfileImageAsync(IFormFile file, string playerEmail)
        {
            try
            {
                if (!IsValidImageFile(file))
                {
                    throw new ArgumentException("Invalid file type or size");
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{SanitizeEmailForFileName(playerEmail)}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_profileImagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Path.Combine("uploads", "profile-images", fileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile image for player {PlayerEmail}", playerEmail);
                throw;
            }
        }

        public Task<bool> DeleteProfileImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return Task.FromResult(true);

                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.Replace("/", "\\"));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Deleted profile image: {ImagePath}", imagePath);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile image: {ImagePath}", imagePath);
                return Task.FromResult(false);
            }
        }

        public string GetProfileImageUrl(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "http://localhost:5145/assets/default-avatar.png";

            return $"http://localhost:5145/{imagePath}";
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        private string SanitizeEmailForFileName(string email)
        {
            return email.Replace("@", "_at_").Replace(".", "_");
        }
    }
}
