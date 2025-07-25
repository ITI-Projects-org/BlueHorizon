import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { Unit } from '../../Models/unit.model';
import { SearchService } from '../../Services/search.service';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { Navbar } from '../../Layout/navbar/navbar';

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

  unitTypeMap: { [key: number]: string } = {
    0: 'Apartment',
    1: 'Villa',
    2: 'Chalet',
  };

  private destroy$ = new Subject<void>();

  constructor(
    private unitsService: UnitsService,
    private route: ActivatedRoute,
    private searchService: SearchService
  ) {}

  ngOnInit(): void {
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
      });

    this.route.queryParams.subscribe((params) => {
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

      this.fetchUnits();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  fetchUnits(): void {
    this.isLoading = true;
    this.unitsService.getUnits().subscribe({
      next: (data) => {
        this.allUnits = data;
        this.isLoading = false;
        this.populateFilterOptions();
        this.applyAllFiltersAndSortAndPaginate();
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
    }

    if (this.selectedVillage) {
      tempUnits = tempUnits.filter(
        (unit) => unit.villageName === this.selectedVillage
      );
    }

    if (this.selectedType) {
      tempUnits = tempUnits.filter(
        (unit) =>
          unit.unitType !== undefined &&
          this.unitTypeMap[unit.unitType] === this.selectedType
      );
    }

    if (this.minPrice !== null) {
      tempUnits = tempUnits.filter(
        (unit) =>
          unit.basePricePerNight !== undefined &&
          unit.basePricePerNight >= this.minPrice!
      );
    }
    if (this.maxPrice !== null) {
      tempUnits = tempUnits.filter(
        (unit) =>
          unit.basePricePerNight !== undefined &&
          unit.basePricePerNight <= this.maxPrice!
      );
    }

    if (this.selectedBedrooms) {
      if (this.selectedBedrooms === '3+') {
        tempUnits = tempUnits.filter(
          (unit) => unit.bedrooms !== undefined && unit.bedrooms >= 3
        );
      } else {
        const num = parseInt(this.selectedBedrooms, 10);
        tempUnits = tempUnits.filter((unit) => unit.bedrooms === num);
      }
    }

    if (this.selectedBathrooms) {
      if (this.selectedBathrooms === '3+') {
        tempUnits = tempUnits.filter(
          (unit) => unit.bathrooms !== undefined && unit.bathrooms >= 3
        );
      } else {
        const num = parseInt(this.selectedBathrooms, 10);
        tempUnits = tempUnits.filter((unit) => unit.bathrooms === num);
      }
    }

    this.filteredUnits = tempUnits;
    this.sortUnits();
    this.calculateTotalPages();
    this.paginate();
  }

  sortUnits(): void {
    switch (this.sortOption) {
      case 'price-asc':
        this.filteredUnits.sort(
          (a, b) => (a.basePricePerNight || 0) - (b.basePricePerNight || 0)
        );
        break;
      case 'price-desc':
        this.filteredUnits.sort(
          (a, b) => (b.basePricePerNight || 0) - (a.basePricePerNight || 0)
        );
        break;
      default:
        this.filteredUnits.sort((a, b) =>
          (a.title || '').localeCompare(b.title || '')
        );
        break;
    }
  }

  calculateTotalPages(): void {
    this.totalPages = Math.ceil(this.filteredUnits.length / this.pageSize);
    if (this.totalPages === 0) this.totalPages = 1;
    if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;
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
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.paginate();
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
    return unit.imagePath && unit.imagePath.length > 0
      ? unit.imagePath
      : 'assets/placeholder.jpg';
  }
}
