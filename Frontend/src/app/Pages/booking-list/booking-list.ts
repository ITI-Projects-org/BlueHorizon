import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router'; // Add Router import
import { BookingService } from '../../Services/booking-service';
import { BookingResponseDTO } from '../../Models/booking-dto';
import { QrCodeDto } from '../../Models/qr-code-dto';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { QrServise } from '../../Services/qr-service';

@Component({
  selector: 'app-booking-list',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-list.html',
  styleUrl: './booking-list.css',
  standalone: true,
})
export class BookingList implements OnInit {
  bookings: BookingResponseDTO[] = [];
  loading = false;
  error: string | null = null;
  showQrModal: boolean = false;
  selectedBooking: BookingResponseDTO | null = null;

  constructor(
    private bookingService: BookingService,
    private qrService: QrServise,
    private cdr: ChangeDetectorRef,
    private router: Router // Add Router to constructor
  ) {}

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
        console.log(bookings);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load bookings. Please try again.';
        this.loading = false;
        console.error('Error loading bookings:', err);
        this.cdr.detectChanges();
      },
    });
  }

  // Fixed: Explicitly type BookingId to accept number or null
  qrCodeDto = new FormGroup({
    BookingId: new FormControl<number | null>(null),
    TenantNationalId: new FormControl('', Validators.required),
    VillageName: new FormControl(''),
    UnitAddress: new FormControl(''),
    OwnerName: new FormControl(''),
    TenantName: new FormControl(''),
  });

  get BookingId() {
    return this.qrCodeDto.controls['BookingId'];
  }
  get TenantNationalId() {
    return this.qrCodeDto.controls['TenantNationalId'];
  }
  get VillageName() {
    return this.qrCodeDto.controls['VillageName'];
  }
  get UnitAddress() {
    return this.qrCodeDto.controls['UnitAddress'];
  }
  get OwnerName() {
    return this.qrCodeDto.controls['OwnerName'];
  }
  get TenantName() {
    return this.qrCodeDto.controls['TenantName'];
  }

  // NEW METHOD: Navigate to QR Creation Component
  navigateToCreateQr(booking: BookingResponseDTO): void {
    // Navigate to create-qr page with booking ID as query parameter
    this.router.navigate(['/create-qr'], { 
      queryParams: { bookingId: booking.id } 
    });
  }

  // KEEP THIS for modal functionality if you want both options
  openQrModal(booking: BookingResponseDTO): void {
    // Reset error state
    this.qrError = null;
    this.isGeneratingQr = false;

    this.selectedBooking = booking;

    if (!booking.unit) {
      this.qrError = 'Cannot create QR code: Unit information is missing';
      return;
    }

    // Pre-fill the form with booking and unit information
    this.qrCodeDto.patchValue({
      BookingId: booking.id,
      VillageName: booking.unit.villageName || '',
      UnitAddress: booking.unit.address || '',
      OwnerName: booking.unit.ownerName || '',
      TenantName: booking.tenantName || '', // Set if available from the booking
      TenantNationalId: '', // Always requires user input
    });

    this.showQrModal = true;
  }

  closeQrModal(): void {
    this.showQrModal = false;
    this.selectedBooking = null;
    this.qrCodeDto.reset();
    this.qrError = null;
    this.isGeneratingQr = false;
  }

  // Loading state for QR code generation
  isGeneratingQr = false;
  qrError: string | null = null;

  submitQrForm(): void {
    if (this.qrCodeDto.valid) {
      this.isGeneratingQr = true;
      this.qrError = null;
      const qrData: QrCodeDto = this.qrCodeDto.value as QrCodeDto;

      this.qrService.createQrCloud(qrData).subscribe({
        next: (response) => {
          if (this.selectedBooking) {
            const index = this.bookings.findIndex(
              (b) => b.id === this.selectedBooking!.id
            );
            if (index !== -1) {
              this.bookings[index].qrCodeUrl = response.imgPath;
              this.cdr.detectChanges();
            }
          }
          this.isGeneratingQr = false;
          this.closeQrModal();
        },
        error: (err) => {
          this.isGeneratingQr = false;
          this.qrError = 'Failed to create QR code. Please try again.';
          console.error('Error creating QR code:', err);
          this.cdr.detectChanges();
        },
      });
    } else {
      this.qrCodeDto.markAllAsTouched();
    }
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
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
    switch (status?.toLowerCase()) {
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

  canCancelBooking(booking: BookingResponseDTO): boolean {
    return (
      this.isUpcoming(booking.checkInDate) &&
      booking.paymentStatus?.toLowerCase() !== 'cancelled'
    );
  }

  trackByBookingId(index: number, booking: BookingResponseDTO): number {
    return booking.id;
  }

  viewBookingDetails(booking: BookingResponseDTO): void {
    console.log('View booking details:', booking);
  }

  cancelBooking(booking: BookingResponseDTO): void {
    console.log('Cancel booking:', booking);
  }

  leaveReview(booking: BookingResponseDTO): void {
    console.log('Leave review for booking:', booking);
  }
}