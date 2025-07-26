import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Unit } from '../../Services/unit';
import { IUnit } from '../../Models/iunit';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { BookingForm } from '../../Pages/booking-form/booking-form';

@Component({
  selector: 'app-unit-details',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatIconModule, BookingForm],
  templateUrl: './unit-details.html',
  styleUrl: './unit-details.css',
})
export class UnitDetailsComponent implements OnInit {
  unit: IUnit | null = null;
  unitId: number = 0;
  loading = true;
  error: string | null = null;
  showBookingForm = false;

  constructor(private route: ActivatedRoute, private unitService: Unit) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.unitId = id;
    if (!id) {
      this.error = 'Invalid unit ID.';
      this.loading = false;
      return;
    }
    this.unitService
      .GetUnitById(id)
      .pipe(
        catchError((err) => {
          this.error = 'Failed to load unit details.';
          this.loading = false;
          return of(null);
        })
      )
      .subscribe((unit) => {
        this.unit = unit;
        this.loading = false;
      });
  }

  openBookingForm(): void {
    this.showBookingForm = true;
  }

  closeBookingForm(): void {
    this.showBookingForm = false;
  }

  onBookingSuccess(response: any): void {
    // Handle successful booking
    alert(
      `Booking successful! Booking ID: ${response.bookingId}, Total: $${response.totalPrice}`
    );
    this.closeBookingForm();
  }
}
