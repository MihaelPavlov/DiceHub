export interface ISpaceTableList {
  id: number;
  tableName: string;
  gameName: string;
  gameImageUrl: string;
  peopleJoined: number;
  maxPeople: number;
  isLocked: boolean;
  createdBy: string;
}
