export interface DelegateOrganizerRoleDto {
  friendUserId: number;
  notes?: string;
}

export interface ReclaimOrganizerRoleDto {
  delegationId: number;
}

export interface OrganizerDelegateDto {
  id: number;
  originalOrganizerId: number;
  originalOrganizerName: string;
  delegateUserId: number;
  delegateUserName: string;
  createdAt: Date;
  reclaimedAt?: Date;
  isActive: boolean;
  notes?: string;
}

export interface DelegationStatusDto {
  isDelegating: boolean;
  isDelegate: boolean;
  currentDelegation?: OrganizerDelegateDto;
  receivedDelegation?: OrganizerDelegateDto;
}
