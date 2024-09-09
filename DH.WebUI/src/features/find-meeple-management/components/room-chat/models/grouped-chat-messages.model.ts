export interface GroupedChatMessage {
  senderId: string;
  senderUsername: string;
  messages: IGroupMessage[];
  dateCreated: Date;
  infoMessages: IGroupMessage[];
}

export interface IGroupMessage {
  text: string;
  dateCreated: Date;
}
