import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Unit } from '../../Services/unit';
import { IUnit } from '../../Models/iunit';
import { CommonModule } from '@angular/common';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { BookingForm } from '../../Pages/booking-form/booking-form';
import { BookingService } from '../../Services/booking-service';
import { BookedSlotsDTO } from '../../Models/booked-slots-dto';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unit-details',
  standalone: true,
  imports: [CommonModule, BookingForm],
  templateUrl: './unit-details.html',
  styleUrl: './unit-details.css',
})
export class UnitDetailsComponent implements OnInit {
  unit: IUnit | null = null;
  unitId: number = 0;
  loading = true;
  error: string | null = null;
  showBookingForm = false;

  // Calendar properties
  bookedSlots: BookedSlotsDTO | null = null;
  calendarLoading = false;
  currentMonth: Date = new Date();
  calendarDays: any[] = [];
  bookedDates: Set<string> = new Set();

  constructor(
    private route: ActivatedRoute,
    private unitService: Unit,
    private bookingService: BookingService,
    private router: Router
  ) {}

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
        console.log('Unit details loaded:', unit);
        this.loading = false;
        
        if (unit) {
          this.loadBookedSlots();
        }
       
      });
  }
  goToChatWithOwner(ownerId: string): void {
    console.log('Navigating to chat with owner:', ownerId);
    if (ownerId) {
      // نستخدم Router.navigate للانتقال إلى مسار /chat
      // ونمرر ownerId كـ query parameter باسم 'userId'
      this.router.navigate(['/chat'], { queryParams: { userId: ownerId } });
      console.log('Navigating to chat with ownerId:', ownerId);
    } else {
      console.warn('Owner ID is not available. Cannot navigate to chat.');
      // يمكنك هنا عرض رسالة للمستخدم (مثلاً باستخدام خدمة Toast/Snackbar)
    }
  }
  loadBookedSlots(): void {
    this.calendarLoading = true;
    this.bookingService
      .getBookedSlots(this.unitId)
      .pipe(
        catchError((err) => {
          console.error('Failed to load booked slots:', err);
          this.calendarLoading = false;
          return of(null);
        })
      )
      .subscribe((bookedSlots) => {
        this.bookedSlots = bookedSlots;
        this.processBookedDates();
        this.generateCalendar();
        this.calendarLoading = false;
      });
  }

  processBookedDates(): void {
    this.bookedDates.clear();
    console.log('Processing booked slots:', this.bookedSlots);

    if (this.bookedSlots?.bookingSlots) {
      this.bookedSlots.bookingSlots.forEach((slot) => {
        console.log('Processing slot:', slot);

        // Create dates and ensure they are in local timezone
        const checkInDate = new Date(slot.checkInDate);
        const checkOutDate = new Date(slot.checkOutDate);

        console.log('Check-in date:', checkInDate);
        console.log('Check-out date:', checkOutDate);

        // Add all dates between check-in and check-out (inclusive)
        const currentDate = new Date(checkInDate);

        // Reset time to avoid timezone issues
        currentDate.setHours(0, 0, 0, 0);
        const endDate = new Date(checkOutDate);
        endDate.setHours(0, 0, 0, 0);

        while (currentDate <= endDate) {
          const dateString = this.formatDateString(currentDate);
          console.log('Adding booked date:', dateString);
          this.bookedDates.add(dateString);
          currentDate.setDate(currentDate.getDate() + 1);
        }
      });
    }

    console.log('Final booked dates set:', Array.from(this.bookedDates));
  }

  generateCalendar(): void {
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();

    // Get first day of the month and number of days in the month
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);

    // Adjust to start from Sunday
    startDate.setDate(startDate.getDate() - firstDay.getDay());

    this.calendarDays = [];
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time for accurate comparison

    console.log('Generating calendar for month:', this.getMonthYearString());
    console.log('Today:', today);
    console.log('Booked dates:', Array.from(this.bookedDates));

    // Generate 42 days (6 weeks) for the calendar
    for (let i = 0; i < 42; i++) {
      const currentDate = new Date(startDate);
      currentDate.setDate(startDate.getDate() + i);
      currentDate.setHours(0, 0, 0, 0); // Reset time for accurate comparison

      const dateString = this.formatDateString(currentDate);
      const isCurrentMonth = currentDate.getMonth() === month;
      const isToday = this.isSameDate(currentDate, today);
      const isPast = currentDate < today;
      const isBooked = this.bookedDates.has(dateString);

      const dayObj = {
        date: currentDate,
        day: currentDate.getDate(),
        isCurrentMonth,
        isToday,
        isPast,
        isBooked,
        isAvailable: isCurrentMonth && !isPast && !isBooked,
      };

      // Debug logging for today and booked dates
      if (isToday) {
        console.log('Today found:', dayObj);
      }
      if (isBooked) {
        console.log('Booked day found:', dayObj, 'dateString:', dateString);
      }

      this.calendarDays.push(dayObj);
    }

    console.log('Calendar days generated:', this.calendarDays.length);
  }

  formatDateString(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  isSameDate(date1: Date, date2: Date): boolean {
    // Reset time components for accurate date comparison
    const d1 = new Date(date1);
    const d2 = new Date(date2);
    d1.setHours(0, 0, 0, 0);
    d2.setHours(0, 0, 0, 0);
    return d1.getTime() === d2.getTime();
  }

  previousMonth(): void {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() - 1,
      1
    );
    this.generateCalendar();
  }

  nextMonth(): void {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + 1,
      1
    );
    this.generateCalendar();
  }

  getMonthYearString(): string {
    return this.currentMonth.toLocaleString('default', {
      month: 'long',
      year: 'numeric',
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
    Swal.fire({
      title: 'Booking Successful!',
      text: 'A confirmation email of the booking was sent, please check it',
      icon: 'success',
      draggable: true,
    }).then(() => {
      this.closeBookingForm();
      this.loadBookedSlots();
    });
  }
}
