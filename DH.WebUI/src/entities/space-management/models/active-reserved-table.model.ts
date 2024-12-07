export interface IActiveReservedTable {
    id: number;
    userId: string;
    username: string;
    numberOfGuests: number;
    reservationDate: Date;
    createdDate: Date;
    isActive: boolean;
}
