export interface IUserActiveSpaceTableResult {
  isPlayerHaveActiveTable: boolean;
  isPlayerParticipateInTable: boolean;
  activeTableName?: string | null;
  activeTableId?: number | null;
}
