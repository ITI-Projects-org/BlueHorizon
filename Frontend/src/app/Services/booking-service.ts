import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { BookingDTO } from '../Models/booking-dto';
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
}
