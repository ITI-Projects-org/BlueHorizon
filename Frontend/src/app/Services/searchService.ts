// src/app/services/searchService.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Unit } from '../Models/unit.model';
import { HttpClient } from '@angular/common/http';


// واجهة لتعريف شكل بيانات معايير البحث
export interface SearchCriteria {
  selectedBedrooms?: string | null;
  selectedBathrooms?: string | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  selectedVillage?: string | null; // ستبقى camelCase لأنها متغير محلي للفلتر
  selectedType?: string | null; // ستبقى string هنا حيث نتعامل معها كاسم
  sortOption?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private searchCriteriaSubject = new BehaviorSubject<SearchCriteria>({
    selectedBedrooms: null,
    selectedBathrooms: null,
    minPrice: null,
    maxPrice: null,
    selectedVillage: null,
    selectedType: null,
    sortOption: 'default'
  });

  searchCriteria$: Observable<SearchCriteria> = this.searchCriteriaSubject.asObservable();

  constructor() { }

  updateSearchCriteria(newCriteria: Partial<SearchCriteria>) {
    const currentCriteria = this.searchCriteriaSubject.getValue();
    const updatedCriteria = { ...currentCriteria, ...newCriteria };
    this.searchCriteriaSubject.next(updatedCriteria);
  }

  clearSearchCriteria() {
    this.searchCriteriaSubject.next({
      selectedBedrooms: null,
      selectedBathrooms: null,
      minPrice: null,
      maxPrice: null,
      selectedVillage: null,
      selectedType: null,
      sortOption: 'default'
    });
  }
}