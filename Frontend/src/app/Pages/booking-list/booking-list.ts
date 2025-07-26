import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../Services/booking-service';
import { BookingResponseDTO } from '../../Models/booking-dto';

@Component({
  selector: 'app-booking-list',
  imports: [CommonModule],
  templateUrl: './booking-list.html',
  styleUrl: './booking-list.css',
  standalone: true,
})
export class BookingList implements OnInit {
  bookings: BookingResponseDTO[] = [];
  loading = false;
  error: string | null = null;

  constructor(private bookingService: BookingService) {}

  ngOnInit() {
    this.loadMyBookings();
  }

  loadMyBookings() {
    this.loading = true;
    this.error = null;

    this.bookingService.getMyBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load bookings. Please try again.';
        this.loading = false;
        console.error('Error loading bookings:', error);
      },
    });
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'success';
      case 'pending':
        return 'warning';
      case 'cancelled':
        return 'danger';
      default:
        return 'secondary';
    }
  }

  getStatusIcon(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'bi-check-circle';
      case 'pending':
        return 'bi-clock';
      case 'cancelled':
        return 'bi-x-circle';
      default:
        return 'bi-question-circle';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  getDaysBetween(checkIn: Date, checkOut: Date): number {
    const startDate = new Date(checkIn);
    const endDate = new Date(checkOut);
    const timeDifference = endDate.getTime() - startDate.getTime();
    return Math.ceil(timeDifference / (1000 * 3600 * 24));
  }

  isUpcoming(checkInDate: Date): boolean {
    return new Date(checkInDate) > new Date();
  }

  isPast(checkOutDate: Date): boolean {
    return new Date(checkOutDate) < new Date();
  }

  trackByBookingId(index: number, booking: BookingResponseDTO): number {
    return booking.id;
  }

  viewBookingDetails(booking: BookingResponseDTO): void {
    // TODO: Navigate to booking details or open modal
    console.log('View booking details:', booking);
  }

  cancelBooking(booking: BookingResponseDTO): void {
    // TODO: Implement cancel booking functionality
    if (confirm('Are you sure you want to cancel this booking?')) {
      console.log('Cancel booking:', booking);
    }
  }

  leaveReview(booking: BookingResponseDTO): void {
    // TODO: Navigate to review form or open modal
    console.log('Leave review for booking:', booking);
  }
}
