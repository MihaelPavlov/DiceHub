export interface IReservedTable {
    id: number;
    userId: string;
    username: string;
    numberOfGuests: number;
    reservationDate: Date;
    createdDate: Date;
    isActive: boolean;
    isReservationSuccessful: boolean;
    isConfirmed: boolean;
}
