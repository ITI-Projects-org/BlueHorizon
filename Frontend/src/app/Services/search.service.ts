// src/app/services/search.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

// واجهة لتعريف شكل بيانات معايير البحث
export interface SearchCriteria {
  title: string | null;
  selectedBedrooms: string | null;
  selectedBathrooms: string | null;
  minPrice: number | null;
  maxPrice: number | null;
  selectedVillage: string | null;
  selectedType: string | null;
}

@Injectable({
  providedIn: 'root', // هذه الخدمة ستكون Singleton على مستوى التطبيق بالكامل
})
export class SearchService {
  // BehaviorSubject: يحتفظ بالقيمة الأخيرة الصادرة، وعند الاشتراك، يصدر القيمة الأخيرة فوراً.
  // القيمة الافتراضية عند البدء هي كل الخصائص null.
  private searchCriteriaSubject = new BehaviorSubject<SearchCriteria>({
    title: null,
    selectedBedrooms: null,
    selectedBathrooms: null,
    minPrice: null,
    maxPrice: null,
    selectedVillage: null,
    selectedType: null,
  });

  // Observable: الجزء الذي تشترك فيه الكومبوننتات الأخرى (مثل UnitsComponent) لتلقي التحديثات.
  searchCriteria$: Observable<SearchCriteria> =
    this.searchCriteriaSubject.asObservable();

  constructor() {}

  /**
   * لتحديث معايير البحث. يتم استدعاؤها من NavbarComponent.
   * تقبل Partial<SearchCriteria> للسماح بتحديث جزء فقط من المعايير.
   */
  updateSearchCriteria(newCriteria: Partial<SearchCriteria>) {
    const currentCriteria = this.searchCriteriaSubject.getValue(); // الحصول على المعايير الحالية
    const updatedCriteria = { ...currentCriteria, ...newCriteria }; // دمج المعايير الجديدة مع القديمة
    this.searchCriteriaSubject.next(updatedCriteria); // إرسال المعايير المحدثة إلى جميع المشتركين
  }

  /**
   * (اختياري) لتصفير جميع معايير البحث.
   */
  clearSearchCriteria() {
    this.searchCriteriaSubject.next({
      title: null,
      selectedBedrooms: null,
      selectedBathrooms: null,
      minPrice: null,
      maxPrice: null,
      selectedVillage: null,
      selectedType: null,
    });
  }
}
