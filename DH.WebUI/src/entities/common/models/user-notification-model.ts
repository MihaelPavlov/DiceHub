export interface IUserNotification {
  id: number;
  messageId: string;
  messageBody: string;
  messageTitle: string;
  messageType: string;
  createdDate: Date;
  hasBeenViewed: boolean;
}
