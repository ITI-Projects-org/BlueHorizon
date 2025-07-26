// app/Pages/home/home.ts
import { Component, OnInit, OnDestroy } from '@angular/core'; // أضف OnInit و OnDestroy
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router'; // أضف ActivatedRoute و Router
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
import { FormsModule } from '@angular/forms'; // استيراد FormsModule لـ [(ngModel)]
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    FormsModule // إضافة FormsModule إلى Imports لتفعيل [(ngModel)]
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy { // تطبيق OnInit و OnDestroy
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // Hero Section - Slider properties (منقولة من الـ Hero Section)
  slides = [
    { image: 'images/1.jpg' }, // تأكد من وجود هذه الصور في مجلد assets
    { image: 'images/3.jpg' },
    { image: 'images/2.jpeg' }
  ];
  currentSlide = 0;
  slideInterval: any;

  // Hero Section - Search form properties (منقولة من الـ Hero Section)
  selectedVillage: string | null = null;
  selectedType: string | null = null;
  selectedBedrooms: string | null = null;
  selectedBathrooms: string | null = null;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  showMoreOptions = false;

  // Dummy data for dropdowns (استبدل بالبيانات الفعلية من خدمة إذا لزم الأمر)
  villages: string[] = ['Village A', 'Village B', 'Village C'];
  unitTypes: string[] = ['Apartment', 'Chalet', 'Villa'];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // بدء السلايدر عند تهيئة المكون
    this.startSlider();
    // قراءة معلمات الاستعلام (query parameters) عند التحميل لملء حقول البحث مسبقًا
    this.activatedRoute.queryParams.subscribe(params => {
      this.selectedVillage = params['village'] || null;
      this.selectedType = params['type'] || null;
      this.selectedBedrooms = params['bedrooms'] || null;
      this.selectedBathrooms = params['bathrooms'] || null;
      this.minPrice = params['minPrice'] ? parseFloat(params['minPrice']) : null;
      this.maxPrice = params['maxPrice'] ? parseFloat(params['maxPrice']) : null;

      // إظهار الخيارات الإضافية إذا تم ملء أي من الفلاتر المتقدمة
      if (this.selectedBedrooms || this.selectedBathrooms || this.minPrice || this.maxPrice) {
        this.showMoreOptions = true;
      }
    });
  }

  ngOnDestroy(): void {
    // إيقاف السلايدر عند تدمير المكون لتجنب تسرب الذاكرة
    this.stopSlider();
  }

  // Slider methods
  startSlider(): void {
    this.stopSlider(); // مسح أي فاصل زمني موجود
    this.slideInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.slides.length;
    }, 5000); // تغيير الشريحة كل 5 ثوانٍ
  }

  stopSlider(): void {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
    this.startSlider(); // إعادة ضبط الفاصل الزمني عند التنقل اليدوي
  }

  // Search form methods
  toggleMoreOptions(event: Event): void {
    event.preventDefault(); // منع سلوك الرابط الافتراضي
    this.showMoreOptions = !this.showMoreOptions;
  }

  applySearchFilters(): void {
    const queryParams: any = {};

    if (this.selectedVillage) {
      queryParams['village'] = this.selectedVillage;
    }
    if (this.selectedType) {
      queryParams['type'] = this.selectedType;
    }
    if (this.selectedBedrooms) {
      queryParams['bedrooms'] = this.selectedBedrooms;
    }
    if (this.selectedBathrooms) {
      queryParams['bathrooms'] = this.selectedBathrooms;
    }
    if (this.minPrice !== null) {
      queryParams['minPrice'] = this.minPrice;
    }
    if (this.maxPrice !== null) {
      queryParams['maxPrice'] = this.maxPrice;
    }

    // التنقل إلى نفس المسار ولكن مع تحديث معلمات الاستعلام
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: queryParams,
      queryParamsHandling: 'merge' // دمج مع أي معلمات استعلام موجودة
    });

    console.log('Applying filters:', queryParams);
    // هنا، عادةً ما تقوم بإرسال إجراء (dispatch an action) أو استدعاء خدمة (call a service)
    // لجلب الوحدات بناءً على هذه الفلاتر، على سبيل المثال: this.unitService.searchUnits(queryParams);
  }
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
