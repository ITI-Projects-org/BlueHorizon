// src/app/services/searchService.ts
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { ISearchCriteria } from "../Models/isearch-criteria";

@Injectable({
  providedIn: "root",
})
export class SearchService {
  private searchCriteriaSubject = new BehaviorSubject<ISearchCriteria>({
    selectedBedrooms: null,
    selectedBathrooms: null,
    minPrice: null,
    maxPrice: null,
    selectedVillage: null,
    selectedType: null,
    sortOption: "default",
  });

  searchCriteria$: Observable<ISearchCriteria> =
    this.searchCriteriaSubject.asObservable();

  constructor() {}

  updateSearchCriteria(newCriteria: Partial<ISearchCriteria>) {
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
      sortOption: "default",
    });
  }
}
