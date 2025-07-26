import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Unit } from '../../Models/unit.model';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { Navbar } from "../../Layout/navbar/navbar";
import { UnitsService } from '../../Services/units.service';
import { SearchService } from '../../Services/searchService';

@Component({
  selector: 'app-units',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './units.html',
  styleUrl: './units.css',
  providers: [UnitsService]
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
    2: 'Chalet'
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
      .subscribe(criteria => {
        this.selectedVillage = criteria.selectedVillage;
        this.selectedType = criteria.selectedType;
        this.selectedBedrooms = criteria.selectedBedrooms;
        this.selectedBathrooms = criteria.selectedBathrooms;
        this.minPrice = criteria.minPrice;
        this.maxPrice = criteria.maxPrice;
        this.currentPage = 1;
        this.applyAllFiltersAndSortAndPaginate();
      });

    this.route.queryParams.subscribe(params => {
      this.selectedVillage = params['village'] || null;
      this.selectedType = params['type'] || null;
      this.selectedBedrooms = params['bedrooms'] || null;
      this.selectedBathrooms = params['bathrooms'] || null;
      this.minPrice = params['minPrice'] ? parseFloat(params['minPrice']) : null;
      this.maxPrice = params['maxPrice'] ? parseFloat(params['maxPrice']) : null;
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
        console.log('Units fetched successfully');
        console.log(data)
      },
      error: (err) => {
        console.error('Error fetching units', err);
        this.error = 'Failed to load units. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  populateFilterOptions(): void {
    const villageMap = new Map<string, number>();
    const unitTypeMap = new Map<string, number>();

    this.allUnits.forEach(unit => {
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
      tempUnits = tempUnits.filter(unit =>
        unit.title?.toLowerCase().includes(lower) ||
        unit.address?.toLowerCase().includes(lower) ||
        unit.villageName?.toLowerCase().includes(lower) ||
        unit.unitType?.toString().includes(lower)
      );
    }

    if (this.selectedVillage) {
      tempUnits = tempUnits.filter(unit => unit.villageName === this.selectedVillage);
    }

    if (this.selectedType) {
      tempUnits = tempUnits.filter(
        unit => unit.unitType !== undefined && this.unitTypeMap[unit.unitType] === this.selectedType
      );
   ge > 1) {
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
    // return unit.imagePath && unit.imagePath.length > 0 ? unit.imagePath : 'assets/placeholder.jpg';
    console.log("this is image paths")
    console.log(unit.imageURL)
    return unit.imageURL??"";}
}