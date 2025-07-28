import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { Unit } from '../../Models/unit.model';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { Navbar } from '../../Layout/navbar/navbar';
import { SearchService } from '../../Services/searchService';

@Component({
  selector: 'app-units',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './units.html',
  styleUrl: './units.css',
})
export class Units implements OnInit, OnDestroy {
  allUnits: Unit[] = [];
  filteredUnits: Unit[] = [];
  paginatedUnits: Unit[] = [];

  isLoading = true;
  error: string | null = null;

  currentPage = 1;
  pageSize = 6;
  totalPages = 1;

  villageOptions: { name: string; count: number }[] = [];
  unitTypeOptions: { name: string; count: number }[] = [];

  selectedVillage?: string | null = null;
  selectedType?: string | null = null;
  selectedBedrooms?: string | null = null;
  selectedBathrooms?: string | null = null;
  minPrice?: number | null = null;
  maxPrice?: number | null = null;
  searchTerm?: string | null = null;
  sortOption?: string = 'default';
  filtersFromHome: boolean = false;

  unitTypeMap: { [key: number]: string } = {
    0: 'Apartment',
    1: 'Chalet',
    2: 'Villa',
  };

  private destroy$ = new Subject<void>();

  constructor(
    private unitsService: UnitsService,
    private route: ActivatedRoute,
    private searchService: SearchService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.fetchUnits();

    this.searchService.searchCriteria$
      .pipe(takeUntil(this.destroy$), debounceTime(200))
      .subscribe((criteria) => {
        this.selectedVillage = criteria.selectedVillage;
        this.selectedType = criteria.selectedType;
        this.selectedBedrooms = criteria.selectedBedrooms;
        this.selectedBathrooms = criteria.selectedBathrooms;
        this.minPrice = criteria.minPrice;
        this.maxPrice = criteria.maxPrice;
        this.currentPage = 1;
        this.applyAllFiltersAndSortAndPaginate();
        this.cdr.detectChanges();
      });

    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        console.log('Query params received:', params);

        // Check if filters are coming from home page
        this.filtersFromHome = !!(
          params['village'] ||
          params['type'] ||
          params['bedrooms'] ||
          params['bathrooms'] ||
          params['minPrice'] ||
          params['maxPrice']
        );

        this.selectedVillage = params['village'] || null;
        this.selectedType = params['type'] || null;
        this.selectedBedrooms = params['bedrooms'] || null;
        this.selectedBathrooms = params['bathrooms'] || null;
        this.minPrice = params['minPrice']
          ? parseFloat(params['minPrice'])
          : null;
        this.maxPrice = params['maxPrice']
          ? parseFloat(params['maxPrice'])
          : null;
        this.searchTerm = params['search'] || null;

      if (this.allUnits.length > 0) {
        this.applyAllFiltersAndSortAndPaginate();
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  fetchUnits(): void {
    this.isLoading = true;
    this.error = null;

    this.unitsService.getUnits().subscribe({
      next: (data) => {
        this.allUnits = data;
        this.isLoading = false;
        this.populateFilterOptions();
        this.applyAllFiltersAndSortAndPaginate();
        console.log('Units fetched successfully');
            console.log(data);
            this.cdr.detectChanges();

      },
      error: (err) => {
        console.error('Error fetching units', err);
        this.error = 'Failed to load units. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  populateFilterOptions(): void {
    const villageMap = new Map<string, number>();
    const unitTypeMap = new Map<string, number>();

    this.allUnits.forEach((unit) => {
      if (unit.villageName) {
        const village = unit.villageName;
        villageMap.set(village, (villageMap.get(village) || 0) + 1);
      }

      if (unit.unitType !== undefined && this.unitTypeMap[unit.unitType]) {
        const unitType = this.unitTypeMap[unit.unitType];
        unitTypeMap.set(unitType, (unitTypeMap.get(unitType) || 0) + 1);
      }
    });

    this.villageOptions = Array.from(villageMap.entries())
      .map(([name, count]) => ({ name, count }))
      .sort((a, b) => a.name.localeCompare(b.name));

    this.unitTypeOptions = Array.from(unitTypeMap.entries())
      .map(([name, count]) => ({ name, count }))
      .sort((a, b) => a.name.localeCompare(b.name));
  }

  applyAllFiltersAndSortAndPaginate(): void {
    let tempUnits = [...this.allUnits];

    if (this.searchTerm) {
      const lower = this.searchTerm.toLowerCase();
      tempUnits = tempUnits.filter(
        (unit) =>
          unit.title?.toLowerCase().includes(lower) ||
          unit.address?.toLowerCase().includes(lower) ||
          unit.villageName?.toLowerCase().includes(lower) ||
          unit.unitType?.toString().includes(lower)
      );
      this.cdr.detectChanges();
    }

    if (this.selectedVillage) {
      tempUnits = tempUnits.filter(
        (unit) => unit.villageName === this.selectedVillage
      );
    }

    if (this.selectedType) {
      const selectedTypeNumber = Object.keys(this.unitTypeMap).find(
        (key) => this.unitTypeMap[parseInt(key)] === this.selectedType
      );
      if (selectedTypeNumber) {
        tempUnits = tempUnits.filter(
          (unit) => unit.unitType === parseInt(selectedTypeNumber)
        );
      }
    }

    if (this.selectedBedrooms) {
      if (this.selectedBedrooms === '4+') {
        tempUnits = tempUnits.filter(unit => (unit.bedrooms ?? 0) >= 4);
      } else {
        const bedrooms = parseInt(this.selectedBedrooms);
        tempUnits = tempUnits.filter(unit => (unit.bedrooms ?? 0) === bedrooms);
      }
    }

    if (this.selectedBathrooms) {
      if (this.selectedBathrooms === '3+') {
        tempUnits = tempUnits.filter(unit => (unit.bathrooms ?? 0) >= 3);
      } else {
        const bathrooms = parseInt(this.selectedBathrooms);
        tempUnits = tempUnits.filter(unit => (unit.bathrooms ?? 0) === bathrooms);
      }
    }

    if (this.minPrice !== null && this.minPrice !== undefined) {
      tempUnits = tempUnits.filter(unit => (unit.basePricePerNight ?? 0) >= this.minPrice!);
    }

    if (this.maxPrice !== null && this.maxPrice !== undefined) {
      tempUnits = tempUnits.filter(unit => (unit.basePricePerNight ?? 0) <= this.maxPrice!);
    }

    this.filteredUnits = tempUnits;
    this.sortUnits();
    this.calculateTotalPages();
    this.paginate();

    console.log('Filter results:', {
      totalUnits: this.allUnits.length,
      filteredUnits: this.filteredUnits.length,
      paginatedUnits: this.paginatedUnits.length,
      currentPage: this.currentPage,
      totalPages: this.totalPages,
    });
  }

  sortUnits(): void {
    switch (this.sortOption) {
      case 'price-low':
        this.filteredUnits.sort(
          (a, b) => (a.basePricePerNight ?? 0) - (b.basePricePerNight ?? 0)
        );
        break;
      case 'price-high':
        this.filteredUnits.sort(
          (a, b) => (b.basePricePerNight ?? 0) - (a.basePricePerNight ?? 0)
        );
        break;
      case 'name':
        this.filteredUnits.sort((a, b) =>
          (a.title || '').localeCompare(b.title || '')
        );
        break;
      case 'location':
        this.filteredUnits.sort((a, b) =>
          (a.villageName || '').localeCompare(b.villageName || '')
        );
        break;
      default:
        break;
    }
  }

  calculateTotalPages(): void {
    this.totalPages = Math.ceil(this.filteredUnits.length / this.pageSize);
  }

  filterByCity(name: string | null, event: Event): void {
    event.preventDefault();
    this.selectedVillage = this.selectedVillage === name ? null : name;
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  filterByType(name: string | null, event: Event): void {
    event.preventDefault();
    this.selectedType = this.selectedType === name ? null : name;
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  sortBy(option: string): void {
    this.sortOption = option;
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  applyPriceFilter(): void {
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  applyBedroomsBathroomsFilter(): void {
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  paginate(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedUnits = this.filteredUnits.slice(start, end);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.paginate();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.paginate();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.paginate();
    }
  }

  getUnitImagePath(unit: Unit): string {
    return unit.imageURL ?? '';
  }

  hasActiveFilters(): boolean {
    return !!(
      this.selectedVillage ||
      this.selectedType ||
      this.selectedBedrooms ||
      this.selectedBathrooms ||
      this.minPrice ||
      this.maxPrice ||
      this.searchTerm
    );
  }

  removeFilter(filterType: string): void {
    switch (filterType) {
      case 'village':
        this.selectedVillage = null;
        break;
      case 'type':
        this.selectedType = null;
        break;
      case 'bedrooms':
        this.selectedBedrooms = null;
        break;
      case 'bathrooms':
        this.selectedBathrooms = null;
        break;
      case 'price':
        this.minPrice = null;
        this.maxPrice = null;
        break;
      case 'search':
        this.searchTerm = null;
        break;
    }
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  clearAllFilters(): void {
    this.selectedVillage = null;
    this.selectedType = null;
    this.selectedBedrooms = null;
    this.selectedBathrooms = null;
    this.minPrice = null;
    this.maxPrice = null;
    this.searchTerm = null;
    this.sortOption = 'default';
    this.currentPage = 1;
    this.applyAllFiltersAndSortAndPaginate();
  }

  viewUnitDetails(unitId: number | null): void {
    if (unitId) {
      this.router.navigate(['/unitDetails', unitId]);
    }
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img) {
      img.src = 'assets/images/default-unit.jpg';
    }
  }
}
