import { Injectable } from '@angular/core';
import { BookingDTO } from '../Models/booking-dto';
import { BookingSlotDTO } from '../Models/booking-slot-dto';
import { BookedSlotsDTO } from '../Models/booked-slots-dto';

@Injectable({
  providedIn: 'root',
})
export class BookingService {
  bookingDTO!: BookingDTO;
  bookingSlotDTO!: BookingSlotDTO;
  bookedSlotDTO!: BookedSlotsDTO;
  baseUrl: string = 'https://localhost:7083/api/Booking';
}
