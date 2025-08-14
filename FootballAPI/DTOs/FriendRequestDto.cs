namespace FootballAPI.DTOs
{
    public class CreateFriendRequestDto
    {
        public string ReceiverEmail { get; set; } = string.Empty;
    }

    public class FriendRequestDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverUsername { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResponsedAt { get; set; }
    }

    public class FriendRequestResponseDto
    {
        public bool Accept { get; set; }
    }
}
