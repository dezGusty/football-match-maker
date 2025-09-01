using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;

namespace FootballAPI.Utils
{
    public class DelegationValidator
    {
        private readonly IUserRepository _userRepository;

        public DelegationValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ValidationResult> ValidateDelegationRequest(int organizerId, int delegateId)
        {
            var result = new ValidationResult();

            var organizer = await _userRepository.GetByIdAsync(organizerId);
            if (organizer == null)
            {
                result.AddError("Organizer not found");
                return result;
            }

            if (organizer.Role != UserRole.ORGANISER)
            {
                result.AddError("User is not an organizer");
                return result;
            }

            var isDelegate = await _userRepository.GetActiveDelegationByDelegateId(organizerId);
            if (isDelegate != null)
            {
                result.AddError("Delegated organizers cannot delegate to others");
                return result;
            }

            var existingDelegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (existingDelegation != null)
            {
                result.AddError("Organizer already has an active delegation");
                return result;
            }

            var delegateUser = await _userRepository.GetByIdAsync(delegateId);
            if (delegateUser == null)
            {
                result.AddError("Delegate user not found");
                return result;
            }

            if (delegateUser.Role != UserRole.PLAYER)
            {
                result.AddError("Delegate must be a player");
                return result;
            }

            var delegateActiveDelegation = await _userRepository.GetActiveDelegationByDelegateId(delegateId);
            if (delegateActiveDelegation != null)
            {
                result.AddError("User is already acting as delegate for another organizer");
                return result;
            }

            var areFriends = await _userRepository.AreFriends(organizerId, delegateId);
            if (!areFriends)
            {
                result.AddError("Users must be friends to delegate organizer role");
                return result;
            }

            var existingOrganizerDelegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (existingOrganizerDelegation?.DelegateUserId == delegateId)
            {
                result.AddError("Circular delegation is not allowed");
                return result;
            }

            result.IsValid = true;
            return result;
        }

        public async Task<ValidationResult> ValidateReclaimRequest(int organizerId, int delegationId)
        {
            var result = new ValidationResult();

            var delegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (delegation == null)
            {
                result.AddError("No active delegation found for this organizer");
                return result;
            }

            if (delegation.Id != delegationId)
            {
                result.AddError("Invalid delegation ID");
                return result;
            }

            if (delegation.OriginalOrganizerId != organizerId)
            {
                result.AddError("Unauthorized: You can only reclaim your own delegation");
                return result;
            }

            result.IsValid = true;
            return result;
        }

        public async Task<ValidationResult> ValidateOrganizerPermissions(int userId, int matchOrganizerId)
        {
            var result = new ValidationResult();

            if (userId == matchOrganizerId)
            {
                result.IsValid = true;
                return result;
            }

            var delegation = await _userRepository.GetActiveDelegationByDelegateId(userId);
            if (delegation != null && delegation.OriginalOrganizerId == matchOrganizerId)
            {
                result.IsValid = true;
                return result;
            }

            result.AddError("User does not have organizer permissions for this match");
            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }

        public string GetErrorsAsString()
        {
            return string.Join(", ", Errors);
        }
    }
}