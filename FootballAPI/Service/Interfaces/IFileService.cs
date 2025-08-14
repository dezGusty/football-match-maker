namespace FootballAPI.Service.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveProfileImageAsync(IFormFile file, string playerEmail);
        Task<bool> DeleteProfileImageAsync(string imagePath);
        string GetProfileImageUrl(string? imagePath);
        bool IsValidImageFile(IFormFile file);
    }
}
