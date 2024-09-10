export interface GroupedChatMessage {
  senderId: string;
  senderUsername: string;
  messages: IGroupMessage[];
  createdDate: Date;
  infoMessages: IGroupMessage[];
}

export interface IGroupMessage {
  text: string;
  createdDate: Date;
}
