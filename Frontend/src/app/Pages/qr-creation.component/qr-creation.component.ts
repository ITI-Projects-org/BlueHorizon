import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router'; // Import ActivatedRoute and Router
import { QrServise } from '../../Services/qr-service'; // Assuming QrService exists
import { QrCodeDto } from '../../Models/qr-code-dto'; // Assuming QrCodeDto model exists
import { BookingResponseDTO } from '../../Models/booking-dto'; // Assuming BookingResponseDTO model exists

@Component({
  selector: 'app-qr-creation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './qr-creation.component.html',
  styleUrl: './qr-creation.component.css',
})
export class QrCreationComponent implements OnInit {
  bookingId: number | null = null;
  selectedBooking: BookingResponseDTO | null = null; // To hold pre-filled data

  qrCodeDto = new FormGroup({
    BookingId: new FormControl<number | null>(null),
    TenantNationalId: new FormControl('', Validators.required),
    VillageName: new FormControl(''),
    UnitAddress: new FormControl(''),
    OwnerName: new FormControl(''),
    TenantName: new FormControl(''), // This control expects tenantName
  });

  isGeneratingQr = false;
  qrError: string | null = null;
  generatedQrCodeUrl: string | null = null; // To display the generated QR code

  constructor(
    private route: ActivatedRoute, // Inject ActivatedRoute to get route parameters
    private router: Router, // Inject Router for navigation
    private qrService: QrServise // Inject QrService
  ) {}

  ngOnInit(): void {
    // Get bookingId from route parameters
    this.route.queryParams.subscribe(params => {
      if (params['bookingId']) {
        this.bookingId = +params['bookingId']; // Convert to number
        this.qrCodeDto.patchValue({ BookingId: this.bookingId });
        // In a real app, you would fetch the full booking details here using bookingId
        // For now, we'll simulate pre-filling based on the idea that booking data
        // would be passed or fetched.
        this.simulateBookingFetch(this.bookingId);
      }
    });
  }

  // Simulate fetching booking details to pre-fill the form
  // In a real application, this would be an actual service call
  private simulateBookingFetch(bookingId: number): void {
    // Example dummy data (ensure tenantName is present here for testing)
    const dummyBookings: BookingResponseDTO[] = [
      {
        id: 1,
        unitId: 101,
        checkInDate: new Date('2025-08-01'),
        checkOutDate: new Date('2025-08-07'),
        numberOfGuests: 2,
        totalPrice: 500,
        platformComission: 50,
        ownerPayoutAmount: 450,
        bookingDate: new Date('2025-07-20'),
        paymentStatus: 'Confirmed',
        qrCodeUrl: "",
        unitReviewed: false,
        ownerReviewd: false,
        tenantId: 'tenant123',
        tenantName: 'John Doe', // This is now correctly present
        unit: {
          id: 101,
          title: 'Cozy Beach House',
          address: '123 Ocean View',
          villageName: 'Seaside Village',
          ownerName: 'Alice Smith',
          unitType: 'House',
          basePricePerNight: 100
        }
      },
      {
        id: 2,
        unitId: 102,
        checkInDate: new Date('2025-09-10'),
        checkOutDate: new Date('2025-09-15'),
        numberOfGuests: 4,
        totalPrice: 800,
        platformComission: 80,
        ownerPayoutAmount: 720,
        bookingDate: new Date('2025-07-22'),
        paymentStatus: 'Pending',
        qrCodeUrl: "",
        unitReviewed: false,
        ownerReviewd: false,
        tenantId: 'tenant124',
        tenantName: 'Jane Doe', // This is now correctly present
        unit: {
          id: 102,
          title: 'Mountain Retreat',
          address: '456 Pine Trail',
          villageName: 'Whispering Pines',
          ownerName: 'Bob Johnson',
          unitType: 'Cabin',
          basePricePerNight: 150
        }
      }
    ];

    this.selectedBooking = dummyBookings.find(b => b.id === bookingId) || null;

    if (this.selectedBooking && this.selectedBooking.unit) {
      this.qrCodeDto.patchValue({
        VillageName: this.selectedBooking.unit.villageName || '',
        UnitAddress: this.selectedBooking.unit.address || '',
        OwnerName: this.selectedBooking.unit.ownerName || '',
        TenantName: this.selectedBooking.tenantName || '', // This line correctly uses tenantName
      });
    } else {
      this.qrError = 'Booking or unit information not found for pre-filling.';
    }
  }


  // Getters for form controls
  get BookingId() { return this.qrCodeDto.controls['BookingId']; }
  get TenantNationalId() { return this.qrCodeDto.controls['TenantNationalId']; }
  get VillageName() { return this.qrCodeDto.controls['VillageName']; }
  get UnitAddress() { return this.qrCodeDto.controls['UnitAddress']; }
  get OwnerName() { return this.qrCodeDto.controls['OwnerName']; }
  get TenantName() { return this.qrCodeDto.controls['TenantName']; }

  submitQrForm(): void {
    if (this.qrCodeDto.valid) {
      this.isGeneratingQr = true;
      this.qrError = null;
      this.generatedQrCodeUrl = null; // Clear previous QR code

      const qrData: QrCodeDto = this.qrCodeDto.value as QrCodeDto;

      this.qrService.createQrCloud(qrData).subscribe({
        next: (response) => {
          this.generatedQrCodeUrl = response.imgPath; 
          this.isGeneratingQr = false;
          
        },
        error: (err) => {
          this.isGeneratingQr = false;
          this.qrError = 'Failed to create QR code. Please try again.';
          console.error('Error creating QR code:', err);
        },
      });
    } else {
      this.qrCodeDto.markAllAsTouched();
    }
  }

  cancel(): void {
    this.router.navigate(['/my-bookings']);
  }
}
