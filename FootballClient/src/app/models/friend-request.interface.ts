export interface CreateFriendRequest {
  receiverEmail: string;
}

export interface FriendRequest {
  id: number;
  senderId: number;
  senderUsername: string;
  senderEmail: string;
  receiverId: number;
  receiverUsername: string;
  receiverEmail: string;
  status: 'Pending' | 'Accepted' | 'Rejected';
  createdAt: string;
  responsedAt?: string;
}

export interface FriendRequestResponse {
  accept: boolean;
}
