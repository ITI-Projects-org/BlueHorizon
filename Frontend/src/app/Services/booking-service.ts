import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { BookingDTO, BookingResponseDTO } from '../Models/booking-dto';
import { BookingSlotDTO } from '../Models/booking-slot-dto';
import { BookedSlotsDTO } from '../Models/booked-slots-dto';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BookingService {
  baseUrl: string = 'https://localhost:7083/api/Booking';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  addBooking(bookingDTO: BookingDTO): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Add`, bookingDTO);
  }

  getBookedSlots(unitId: number): Observable<BookedSlotsDTO> {
    return this.http.get<BookedSlotsDTO>(
      `${this.baseUrl}/booked-slots/${unitId}`
    );
  }

  getMyBookings(): Observable<BookingResponseDTO[]> {
    return this.http.get<BookingResponseDTO[]>(`${this.baseUrl}/my-bookings`);
  }

  cancelBooking(bookingId: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/delete/${bookingId}`);
  }
}
