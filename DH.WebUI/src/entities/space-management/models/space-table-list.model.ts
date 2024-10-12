export interface ISpaceTableList {
  id: number;
  tableName: string;
  gameName: string;
  gameImageId: number;
  peopleJoined: number;
  maxPeople: number;
  isLocked: boolean;
  createdBy: string;
}
