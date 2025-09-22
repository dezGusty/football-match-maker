using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class DelegateOrganizerRoleDto
    {
        public int FriendUserId { get; set; }
        public string? Notes { get; set; }
    }

    public class ReclaimOrganizerRoleDto
    {
        public int DelegationId { get; set; }
    }

    public class OrganizerDelegateDto
    {
        public int Id { get; set; }
        public int OriginalOrganizerId { get; set; }
        public required string OriginalOrganizerName { get; set; }
        public int DelegateUserId { get; set; }
        public required string DelegateUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReclaimedAt { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class DelegationStatusDto
    {
        public bool IsDelegating { get; set; }
        public bool IsDelegate { get; set; }
        public OrganizerDelegateDto? CurrentDelegation { get; set; }
        public OrganizerDelegateDto? ReceivedDelegation { get; set; }
    }
}