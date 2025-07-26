import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../Services/booking-service';
import { BookingDTO } from '../../Models/booking-dto';

@Component({
  selector: 'app-booking-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-form.html',
  styleUrl: './booking-form.css',
})
export class BookingForm implements OnInit {
  @Input() unitId!: number;
  @Input() isVisible: boolean = true;
  @Output() closeForm = new EventEmitter<void>();
  @Output() bookingSuccess = new EventEmitter<any>();

  bookingForm!: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];

    this.bookingForm = this.fb.group({
      checkInDate: ['', [Validators.required]],
      checkOutDate: ['', [Validators.required]],
      numberOfGuests: [
        1,
        [Validators.required, Validators.min(1), Validators.max(20)],
      ],
    });

    // Set minimum date to today for both check-in and check-out
    this.bookingForm
      .get('checkInDate')
      ?.valueChanges.subscribe((checkInDate) => {
        if (checkInDate) {
          this.bookingForm
            .get('checkOutDate')
            ?.setValidators([Validators.required]);
          this.bookingForm.get('checkOutDate')?.updateValueAndValidity();
        }
      });
  }

  onSubmit(): void {
    if (this.bookingForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      this.errorMessage = '';

      const formValue = this.bookingForm.value;

      // Validate that check-out is after check-in
      const checkIn = new Date(formValue.checkInDate);
      const checkOut = new Date(formValue.checkOutDate);

      if (checkOut <= checkIn) {
        this.errorMessage = 'Check-out date must be after check-in date';
        this.isSubmitting = false;
        return;
      }

      const bookingDTO: BookingDTO = {
        UnitId: this.unitId,
        CheckInDate: checkIn,
        CheckOutDate: checkOut,
        NumberOfGuests: formValue.numberOfGuests,
      };

      this.bookingService.addBooking(bookingDTO).subscribe({
        next: (response) => {
          this.isSubmitting = false;
          this.bookingSuccess.emit(response);
          this.closeModal();
        },
        error: (error) => {
          this.isSubmitting = false;
          this.errorMessage =
            error.error?.msg || 'An error occurred while booking';
        },
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.bookingForm.controls).forEach((key) => {
      this.bookingForm.get(key)?.markAsTouched();
    });
  }

  closeModal(): void {
    this.bookingForm.reset();
    this.errorMessage = '';
    this.isSubmitting = false;
    this.closeForm.emit();
  }

  // Getter methods for template
  get checkInDate() {
    return this.bookingForm.get('checkInDate');
  }
  get checkOutDate() {
    return this.bookingForm.get('checkOutDate');
  }
  get numberOfGuests() {
    return this.bookingForm.get('numberOfGuests');
  }

  // Get today's date in YYYY-MM-DD format for min attribute
  get today(): string {
    return new Date().toISOString().split('T')[0];
  }

  // Get tomorrow's date for minimum checkout
  get tomorrow(): string {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  }
}