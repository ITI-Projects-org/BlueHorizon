import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http'; // فقط استيراد HttpClient
import { RouterModule } from '@angular/router';

export interface Unit {
  id: number;
  title: string;
  image: string;
  address: string;
  price: number;
  beds: number;
  baths: number;
}

@Component({
  selector: 'app-units',
  standalone: true, 
  imports: [CommonModule, FormsModule, RouterModule], 
  templateUrl: './units.html',
  styleUrl: './units.css'
})
export class Units implements OnInit {
  units: Unit[] = []; // ستبدأ فارغة ويتم ملؤها من الـ API
  paginatedUnits: Unit[] = []; // الوحدات المعروضة في الصفحة الحالية

  // Pagination properties
  pageSize: number = 6;
  currentPage: number = 1;
  totalPages: number = 1; // تم تعديل هذا المتغير وتصحيحه

  // بيانات الفلاتر (تم توحيد الهيكل)
  unitType = [
    { name: 'Apartment', count: 0 }, // تم تصحيح الاسم ليكون موحدًا
    { name: 'Villa', count: 0 },
    { name: 'House', count: 0 },
    { name: 'Office', count: 0 },
    { name: 'Land', count: 0 }
  ];

  village = [
    { name: 'Village A', count: 0 },
    { name: 'Village B', count: 0 },
    { name: 'Village C', count: 0 }
  ];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchUnits();
    // this.calculateCounts() سيتم استدعاؤها بعد جلب الوحدات
  }

  fetchUnits(): void {
    // استبدل هذا الرابط برابط الـ API الفعلي الخاص بك
    const apiUrl = 'YOUR_API_ENDPOINT_HERE/units'; // مثال: 'https://api.example.com/units'
    
    this.http.get<Unit[]>(apiUrl).subscribe({
      next: (data) => {
        this.units = data;
        this.calculatePagination(); // حساب الصفحات بعد جلب البيانات
        this.updatePaginatedUnits(); // تحديث الوحدات المعروضة
        this.calculateCounts(); // حساب العدادات للفلاتر بعد جلب البيانات
      },
      error: (errorRes) => { // تغيير اسم المتغير لتجنب التعارض مع استيراد خاطئ سابق
        console.error('Error fetching units:', errorRes);
        // يمكنك إضافة رسالة خطأ مرئية للمستخدم هنا
      },
      complete: () => {
        console.log('Units fetched successfully');
      }
    });
  }

  calculatePagination(): void {
    // يحسب العدد الإجمالي للصفحات
    this.totalPages = Math.ceil(this.units.length / this.pageSize);
  }

  updatePaginatedUnits(): void {
    // تحديث الوحدات المعروضة في الصفحة الحالية
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedUnits = this.units.slice(startIndex, endIndex);
  }

  calculateCounts(): void {
    // يتم حساب عدد الوحدات لكل نوع (Apartment, Villa, House, etc.)
    this.unitType.forEach(type => {
      type.count = this.units.filter(unit => unit.title.toLowerCase().includes(type.name.toLowerCase())).length;
    });

    // يتم حساب عدد الوحدات لكل قرية بناءً على العنوان
    this.village.forEach(village => {
      village.count = this.units.filter(unit => unit.address.toLowerCase().includes(village.name.toLowerCase())).length;
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePaginatedUnits();
    }
  }

  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }

  previousPage(): void {
    this.goToPage(this.currentPage - 1);
  }

  filterByCity(city: string): void {
    console.log('Filtering by city:', city);
    // هنا يجب إضافة منطق الفلترة الفعلي
    // مثال:
    // const filteredUnits = this.units.filter(unit => unit.address.toLowerCase().includes(city.toLowerCase()));
    // this.paginatedUnits = filteredUnits.slice(0, this.pageSize); // عرض الصفحة الأولى من الفلترة
    // this.currentPage = 1;
    // this.calculatePagination(); // قد تحتاج إلى إعادة حساب totalPages بناءً على الفلترة
  }

  filterByType(type: string): void {
    console.log('Filtering by type:', type);
    // منطق الفلترة
  }

  filterByStatus(status: string): void {
    console.log('Filtering by status:', status);
    // منطق الفلترة
  }

  sortBy(criteria: string): void {
    console.log('Sorting by:', criteria);
    // منطق الفرز
    // مثال:
    // if (criteria === 'price-asc') {
    //   this.units.sort((a, b) => a.price - b.price);
    // } else if (criteria === 'price-desc') {
    //   this.units.sort((a, b) => b.price - a.price);
    // }
    // this.updatePaginatedUnits();
  }
}