import { ReservationStatus } from "../../../shared/enums/reservation-status.enum";

export interface IReservedTable {
    id: number;
    userId: string;
    username: string;
    numberOfGuests: number;
    reservationDate: Date;
    createdDate: Date;
    isActive: boolean;
    isReservationSuccessful: boolean;
    status: ReservationStatus;
}
