import { ChangeDetectorRef, Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router'; // Add RouterModule import
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
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-booking-list',
  imports: [CommonModule, ReactiveFormsModule, NgxSpinnerModule, RouterModule],
  templateUrl: './booking-list.html',
  styleUrl: './booking-list.css',
  standalone: true,
})
export class BookingList implements OnInit, OnDestroy {
  bookings: BookingResponseDTO[] = [];
  loading = false;
  error: string | null = null;
  showQrModal: boolean = false;
  selectedBooking: BookingResponseDTO | null = null;

  // Subject for managing component destruction
  private destroy$ = new Subject<void>();

  constructor(
    private bookingService: BookingService,
    private qrService: QrServise,
    private cdr: ChangeDetectorRef,
    private router: Router, // Add Router to constructor
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit() {
    this.loadMyBookings();
    this.cdr.detectChanges();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMyBookings() {
    this.loading = true;
    this.error = null;

    // Trigger change detection to show loading state immediately
    this.cdr.detectChanges();

    this.bookingService
      .getMyBookings()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (bookings) => {
          this.bookings = bookings || []; // Ensure bookings is always an array
          this.loading = false;
          console.log('Bookings loaded:', bookings);
          console.log('Number of bookings:', this.bookings.length);

          // Force change detection to update UI immediately
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.error = 'You do not have any bookings yet.';
          this.loading = false;
          this.bookings = []; // Reset bookings on error
          console.error('Error loading bookings:', err);

          // Force change detection to show error state immediately
          this.cdr.detectChanges();
        },
      });
  }

  // Method to manually refresh bookings with user feedback
  refreshBookings(): void {
    console.log('Manually refreshing bookings...');
    this.loadMyBookings();
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

    // Force change detection when modal closes
    this.cdr.detectChanges();
  }

  // Loading state for QR code generation
  isGeneratingQr = false;
  qrError: string | null = null;

  submitQrForm(): void {
    if (this.qrCodeDto.valid) {
      this.isGeneratingQr = true;
      this.qrError = null;
      const qrData: QrCodeDto = this.qrCodeDto.value as QrCodeDto;

      this.qrService
        .createQrCloud(qrData)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            if (this.selectedBooking) {
              const index = this.bookings.findIndex(
                (b) => b.id === this.selectedBooking!.id
              );
              if (index !== -1) {
                this.bookings[index].qrCodeUrl = response.imgPath;
                console.log(
                  'QR Code generated and added to booking:',
                  this.bookings[index]
                );

                // Force change detection to show QR code immediately
                this.cdr.detectChanges();
              }
            }
            this.isGeneratingQr = false;
            this.closeQrModal();

            // Show success message
            Swal.fire({
              title: 'Success!',
              text: 'QR Code generated successfully',
              icon: 'success',
              timer: 2000,
              showConfirmButton: false,
              draggable: true,
            });
          },
          error: (err) => {
            this.isGeneratingQr = false;
            this.qrError = 'Failed to create QR code. Please try again.';
            console.error('Error creating QR code:', err);

            // Force change detection to show error immediately
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

  // Helper method to check if bookings list is empty
  hasBookings(): boolean {
    return this.bookings && this.bookings.length > 0;
  }

  // Helper method to check if we should show empty state
  shouldShowEmptyState(): boolean {
    return !this.loading && !this.error && !this.hasBookings();
  }

  // Helper method to check if we should show bookings list
  shouldShowBookingsList(): boolean {
    return !this.loading && !this.error && this.hasBookings();
  }

  viewBookingDetails(booking: BookingResponseDTO): void {
    console.log('View booking details:', booking);
  }

  cancelBooking(booking: BookingResponseDTO): void {
    console.log('Cancel booking:', booking);
  }

  confirmCancelBooking(booking: BookingResponseDTO): void {
    Swal.fire({
      title: 'Cancel Booking',
      text: `Are you sure you want to cancel your booking for "${
        booking.unit?.title || 'this property'
      }"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#dc3545',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Yes, Cancel Booking',
      cancelButtonText: 'Keep Booking',
      reverseButtons: true,
      draggable: true,
      customClass: {
        confirmButton: 'btn btn-danger',
        cancelButton: 'btn btn-secondary',
      },
    }).then((result) => {
      if (result.isConfirmed) {
        this.OnCancelBooking(booking.id);
      }
    });
  }

  leaveReview(booking: BookingResponseDTO): void {
    console.log('Leave review for booking:', booking);
  }

  OnCancelBooking(bookingId: number): void {
    this.spinner.show();
    this.bookingService
      .cancelBooking(bookingId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.spinner.hide();
          Swal.fire({
            title: 'Success',
            text: res.msg,
            icon: 'success',
            draggable: true,
          }).then(() => {
            // Refresh the bookings list after successful cancellation
            console.log('Booking cancelled successfully, refreshing list...');
            this.loadMyBookings();
          });
        },
        error: (error) => {
          this.spinner.hide();
          Swal.fire({
            title: 'Error',
            text:
              error.error?.msg ||
              'An error occurred during canceling your booking',
            icon: 'error',
            draggable: true,
          });
          console.log('Error cancelling booking:', error);

          // Force change detection even on error
          this.cdr.detectChanges();
        },
      });
  }
}
