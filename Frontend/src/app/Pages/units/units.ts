import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { Unit } from '../../Models/unit.model';
import { SearchService } from '../../Services/search.service'; // Using standard naming convention
import { Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-units',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './units.html',
  styleUrl: './units.css', // Keeping styleUrl for a single CSS file
  providers: [UnitsService], // Providing UnitsService at the component level
})
// <<<<<<< dev-units
// export class Units implements OnInit {
//   units: IUnit[] = []; // ستبدأ فارغة ويتم ملؤها من الـ API
//   paginatedUnits: IUnit[] = []; // الوحدات المعروضة في الصفحة الحالية

//   // Pagination properties
//   pageSize: number = 6;
//   currentPage: number = 1;
//   totalPages: number = 1; // تم تعديل هذا المتغير وتصحيحه

//   // بيانات الفلاتر (تم توحيد الهيكل)
//   unitType = [
//     { name: 'Apartment', count: 0 }, // تم تصحيح الاسم ليكون موحدًا
//     { name: 'Villa', count: 0 },
//     { name: 'House', count: 0 },
//     { name: 'Office', count: 0 },
//     { name: 'Land', count: 0 },
//   ];

//   village = [
//     { name: 'Village A', count: 0 },
//     { name: 'Village B', count: 0 },
//     { name: 'Village C', count: 0 },
//   ];

//   constructor(private http: HttpClient,private cdr:ChangeDetectorRef) {}
// =======
export class Units implements OnInit, OnDestroy {
  // Data Properties
  allUnits: Unit[] = [];
  filteredUnits: Unit[] = [];
  paginatedUnits: Unit[] = [];

  // State Properties
  isLoading = true;
  error: string | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 6;
  totalPages = 1;

  // Filter Options
  villageOptions: { name: string; count: number }[] = [];
  unitTypeOptions: { name: string; count: number }[] = [];

  // Filter Parameters
  selectedVillage: string | null = null;
  selectedType: string | null = null;
  selectedBedrooms: string | null = null;
  selectedBathrooms: string | null = null;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  searchTerm: string = ''; // Added search term for filtering
  sortOption: string = 'default';

  private destroy$ = new Subject<void>();

  constructor(
    private unitsService: UnitsService,
    private route: ActivatedRoute,
    private searchService: SearchService
  ) {}
// >>>>>>> dev

  ngOnInit(): void {
    // Subscribe to query parameters to apply filters from URL
    this.route.queryParams.subscribe((params) => {
      this.selectedVillage = params['village'] || null;
      this.selectedType = params['unitType'] || null;
      this.selectedBedrooms = params['bedrooms'] || null;
      this.selectedBathrooms = params['bathrooms'] || null;
      this.minPrice = params['minPrice'] ? +params['minPrice'] : null;
      this.maxPrice = params['maxPrice'] ? +params['maxPrice'] : null;
      this.searchTerm = params['search'] || ''; // Update search term from query params

      // Fetch units only if not already loaded, then apply filters
      if (this.allUnits.length === 0) {
        this.fetchAllUnits();
      } else {
        this.applyAllFiltersAndSortAndPaginate();
      }
    });

    // Subscribe to search criteria from SearchService with debounceTime
    this.searchService.searchCriteria$
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300) // Debounce time to prevent rapid filter updates
      )
      .subscribe((criteria) => {
        console.log('Received search criteria:', criteria);
        if (criteria) {
          this.updateFiltersFromCriteria(criteria); // Update filters based on search criteria
          this.applyAllFiltersAndSortAndPaginate(); // Reapply filters
        }
      });
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions to prevent memory leaks
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Updates filter parameters from the received search criteria.
   * @param criteria The search criteria object.
   */
  private updateFiltersFromCriteria(criteria: any): void {
    this.selectedVillage = criteria.selectedVillage;
    this.selectedType = criteria.selectedType;
    this.selectedBedrooms = criteria.selectedBedrooms;
    this.selectedBathrooms = criteria.selectedBathrooms;
    this.minPrice = criteria.minPrice;
    this.maxPrice = criteria.maxPrice;
    this.searchTerm = criteria.title || ''; // Use 'title' for searchTerm from criteria
  }

// <<<<<<< dev-units
  fetchUnits(): void {
    // استبدل هذا الرابط برابط الـ API الفعلي الخاص بك
    const apiUrl = 'https://localhost:7083/api/Unit/All'; // مثال: 'https://api.example.com/units'
// =======
//   /**
//    * Fetches all units from the UnitsService.
//    * Handles loading state and errors.
//    */
//   fetchAllUnits(): void {
//     this.isLoading = true;
//     this.error = null;
// >>>>>>> dev

    this.unitsService.getUnits().subscribe({
      next: (data) => {
// <<<<<<< dev-units
        console.log('data come from request')
        console.log(data)
        this.units = data;
        this.calculatePagination(); // حساب الصفحات بعد جلب البيانات
        this.updatePaginatedUnits(); // تحديث الوحدات المعروضة
        this.calculateCounts(); // حساب العدادات للفلاتر بعد جلب البيانات
// =======
//         this.allUnits = [...data]; // Create a new array reference
//         console.log('Initial units loaded:', this.allUnits);
//         this.extractFilterOptions(this.allUnits); // Extract filter options from all units
//         this.applyAllFiltersAndSortAndPaginate(); // Apply initial filters and pagination
//         this.isLoading = false;
// >>>>>>> dev
      },
      error: (err) => {
        console.error('Failed to load units:', err);
        this.error = 'Failed to load units. Please try again later.';
        this.isLoading = false;
        // Clear unit data on error
        this.allUnits = [];
        this.filteredUnits = [];
        this.paginatedUnits = [];
      },
    });
  }

  /**
   * Extracts unique villages and unit types along with their counts
   * from the provided list of units for filter options.
   * @param unitsToProcess The array of units to extract options from.
   */
  extractFilterOptions(unitsToProcess: Unit[]): void {
    const villageMap: { [key: string]: number } = {};
    const typeMap: { [key: string]: number } = {};

    unitsToProcess.forEach((unit) => {
      if (unit.village) {
        villageMap[unit.village] = (villageMap[unit.village] || 0) + 1;
      }
      if (unit.UnitType) {
        // Assuming UnitType is PascalCase as per Unit.cs DTO
        typeMap[unit.UnitType] = (typeMap[unit.UnitType] || 0) + 1;
      }
    });

// <<<<<<< dev-units
  updatePaginatedUnits(): void {
    // تحديث الوحدات المعروضة في الصفحة الحالية
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedUnits = this.units.slice(startIndex, endIndex);
    this.cdr.detectChanges();
  }

  calculateCounts(): void {
    // يتم حساب عدد الوحدات لكل نوع (Apartment, Villa, House, etc.)
    this.unitType.forEach((type) => {
      type.count = this.units.filter((unit) =>
        unit.title?.toLowerCase().includes(type.name.toLowerCase())
      ).length;
    });

    // يتم حساب عدد الوحدات لكل قرية بناءً على العنوان
    this.village.forEach((village) => {
      village.count = this.units.filter((unit) =>
        unit.address?.toLowerCase().includes(village.name.toLowerCase())
      ).length;
    });
// =======
//     this.villageOptions = Object.keys(villageMap).map((name) => ({
//       name,
//       count: villageMap[name],
//     }));
//     this.unitTypeOptions = Object.keys(typeMap).map((name) => ({
//       name,
//       count: typeMap[name],
//     }));
//   }

//   /**
//    * Applies all selected filters, sorting, and then pagination to the units.
//    */
//   applyAllFiltersAndSortAndPaginate(): void {
//     console.log('Applying filters with:', {
//       searchTerm: this.searchTerm,
//       bedrooms: this.selectedBedrooms,
//       bathrooms: this.selectedBathrooms,
//       village: this.selectedVillage,
//       type: this.selectedType,
//       minPrice: this.minPrice,
//       maxPrice: this.maxPrice,
//     });

//     let tempUnits = [...this.allUnits];
//     console.log('Initial units count for filtering:', tempUnits.length);

//     // Apply search term filter
//     if (this.searchTerm) {
//       const term = this.searchTerm.toLowerCase().trim();
//       tempUnits = tempUnits.filter(
//         (unit) =>
//           unit.title?.toLowerCase().includes(term) ||
//           unit.address?.toLowerCase().includes(term) ||
//           unit.village?.toLowerCase().includes(term) ||
//           unit.UnitType?.toLowerCase().includes(term)
//       );
//       console.log('After search filter:', tempUnits.length);
//     }

//     // Apply village filter
//     if (this.selectedVillage) {
//       tempUnits = tempUnits.filter(
//         (unit) =>
//           unit.village?.toLowerCase() === this.selectedVillage?.toLowerCase()
//       );
//       console.log('After village filter:', tempUnits.length);
//     }

//     // Apply unit type filter
//     if (this.selectedType) {
//       tempUnits = tempUnits.filter(
//         (unit) =>
//           unit.UnitType?.toLowerCase() === this.selectedType?.toLowerCase()
//       );
//       console.log('After type filter:', tempUnits.length);
//     }

//     // Apply bedrooms filter
//     if (this.selectedBedrooms) {
//       const bedroomsNum = parseInt(this.selectedBedrooms);
//       if (!isNaN(bedroomsNum)) {
//         if (this.selectedBedrooms.endsWith('+')) {
//           tempUnits = tempUnits.filter(
//             (unit) => (unit.bedrooms ?? 0) >= bedroomsNum
//           );
//         } else {
//           tempUnits = tempUnits.filter((unit) => unit.bedrooms === bedroomsNum);
//         }
//       }
//       console.log(
//         'After bedrooms filter:',
//         tempUnits.length,
//         'with criteria:',
//         this.selectedBedrooms
//       );
//       console.log(
//         'Matching units:',
//         tempUnits.map((u) => ({ id: u.id, bedrooms: u.bedrooms }))
//       );
//     }

//     // Apply bathrooms filter
//     if (this.selectedBathrooms) {
//       const bathroomsNum = parseInt(this.selectedBathrooms);
//       if (!isNaN(bathroomsNum)) {
//         if (this.selectedBathrooms.endsWith('+')) {
//           tempUnits = tempUnits.filter(
//             (unit) => (unit.bathrooms ?? 0) >= bathroomsNum
//           );
//         } else {
//           tempUnits = tempUnits.filter(
//             (unit) => unit.bathrooms === bathroomsNum
//           );
//         }
//       }
//       console.log('After bathrooms filter:', tempUnits.length);
//     }

//     // Apply price range filter
//     if (this.minPrice !== null) {
//       tempUnits = tempUnits.filter(
//         (unit) =>
//           unit.basePricePerNight !== null &&
//           unit.basePricePerNight! >= this.minPrice!
//       );
//     }
//     if (this.maxPrice !== null) {
//       tempUnits = tempUnits.filter(
//         (unit) =>
//           unit.basePricePerNight !== null &&
//           unit.basePricePerNight! <= this.maxPrice!
//       );
//     }
//     console.log('After price filter:', tempUnits.length);

//     this.filteredUnits = tempUnits;

//     // Apply sorting
//     this.applySorting();

//     // Update and apply pagination
//     this.updatePagination();

//     console.log('Final filtered units count:', this.filteredUnits.length);
//     console.log('Paginated units:', this.paginatedUnits);
// >>>>>>> dev
  }

  /**
   * Applies the selected sort option to the filtered units.
   */
  private applySorting(): void {
    if (this.sortOption === 'price-asc') {
      this.filteredUnits.sort(
        (a, b) => (a.basePricePerNight ?? 0) - (b.basePricePerNight ?? 0)
      );
    } else if (this.sortOption === 'price-desc') {
      this.filteredUnits.sort(
        (a, b) => (b.basePricePerNight ?? 0) - (a.basePricePerNight ?? 0)
      );
    }
  }

  /**
   * Updates total pages and current page based on filtered units.
   */
  private updatePagination(): void {
    this.totalPages = Math.ceil(this.filteredUnits.length / this.pageSize) || 1;
    // Ensure currentPage is within valid bounds
    if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }
    if (this.currentPage < 1 && this.filteredUnits.length > 0) {
      this.currentPage = 1;
    } else if (this.filteredUnits.length === 0) {
      this.currentPage = 1; // If no units, stay on page 1
    }
    this.paginate();
  }

  /**
   * Paginates the filtered units to display the current page.
   */
  paginate(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedUnits = this.filteredUnits.slice(start, end);
  }

  // Public methods for template interaction

  /**
   * Toggles the selected village filter and reapplies all filters.
   * @param name The village name to filter by.
   */
  filterByCity(name: string): void {
    this.selectedVillage = this.selectedVillage === name ? null : name;
    this.currentPage = 1; // Reset to first page
    this.applyAllFiltersAndSortAndPaginate();
  }

  /**
   * Toggles the selected unit type filter and reapplies all filters.
   * @param name The unit type name to filter by.
   */
  filterByType(name: string): void {
    this.selectedType = this.selectedType === name ? null : name;
    this.currentPage = 1; // Reset to first page
    this.applyAllFiltersAndSortAndPaginate();
  }

  /**
   * Sets the sorting option and reapplies all filters.
   * @param option The sorting option ('price-asc', 'price-desc', 'default').
   */
  sortByOption(option: string): void {
    this.sortOption = option;
    this.currentPage = 1; // Reset to first page
    this.applyAllFiltersAndSortAndPaginate();
  }

  /**
   * Navigates to a specific page.
   * @param page The page number to navigate to.
   */
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return; // Prevent invalid page navigation
    this.currentPage = page;
    this.paginate();
  }

  /**
   * Navigates to the previous page.
   */
  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.paginate();
    }
  }

  /**
   * Navigates to the next page.
   */
  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.paginate();
    }
  }

// <<<<<<< dev-units
  sortBy(criteria: string): void {
    console.log('Sorting by:', criteria);
    
    if (criteria === 'price-asc') {
      console.log('insde sortingggggg')
     this.units = this.units.sort((a, b) => a.basePricePerNight - b.basePricePerNight);
    } else if (criteria === 'price-desc') {
      console.log('ninsede elseeeee')

      this.units.sort((a, b) => b.basePricePerNight - a.basePricePerNight);
    
    this.updatePaginatedUnits();
    
// =======
//   /**
//    * TrackBy function for optimizing ngFor performance in templates.
//    * @param index The index of the item.
//    * @param unit The unit object.
//    * @returns The unit's ID.
//    */
//   trackByUnitId(index: number, unit: Unit): number {
//     return unit.id;
// >>>>>>> dev
  }
}
}