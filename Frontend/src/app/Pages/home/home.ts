// app/Pages/home/home.ts
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  OnDestroy,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import {
  ActivatedRoute,
  InitialNavigation,
  RouterLink,
  Router,
} from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { SearchService } from '../../Services/searchService';
import { Unit } from '../../Models/unit.model';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  // Hero Section - Slider properties
  slides = [
    { image: 'images/1.jpg' },
    { image: 'images/3.jpg' },
    { image: 'images/2.jpeg' },
  ];
  currentSlide = 0;
  slideInterval: any;
  private isBrowser: boolean;

  // Hero Section - Search form properties
  selectedVillage: string | null = null;
  selectedType: string | null = null;
  selectedBedrooms: string | null = null;
  selectedBathrooms: string | null = null;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  showMoreOptions = false;

  // Dynamic data for dropdowns
  villages: string[] = [];
  unitTypes: string[] = ['Apartment', 'Chalet', 'Villa'];

  constructor(
    private router: Router,
    private unitsService: UnitsService,
    private activatedRoute: ActivatedRoute,
    private searchService: SearchService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  filteredUnits: Unit[] = [];
  paginatedUnits: Unit[] = [];
  allUnits: Unit[] = [];

  // Properties carousel properties
  currentPropertyPage = 0;
  propertiesPerPage = 3;
  totalPropertyPages = 0;
  maxUnitsToShow = 9;

  isLoading = true;
  isSearching = false;
  error: string | null = null;
  searchPerformed = false;

  ngOnInit(): void {
    this.fetchUnits();
    if (this.isBrowser) {
      this.startSlider();
    }

    this.activatedRoute.queryParams.subscribe((params) => {
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

      if (
        this.selectedBedrooms ||
        this.selectedBathrooms ||
        this.minPrice ||
        this.maxPrice
      ) {
        this.showMoreOptions = true;
      }

      // Apply filters when parameters change
      this.applyFilters();
    });
  }

  ngOnDestroy(): void {
    if (this.isBrowser) {
      this.stopSlider();
    }
  }

  fetchUnits(): void {
    this.isLoading = true;
    this.error = null;

    this.unitsService.getUnits().subscribe({
      next: (data) => {
        this.allUnits = data;
        this.isLoading = false;
        this.populateVillages();
        this.applyFilters();
        console.log('Units fetched successfully:', data.length);
      },
      error: (err) => {
        console.error('Error fetching units', err);
        this.error = 'Failed to load units. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  populateVillages(): void {
    // Extract unique villages from units
    const villageSet = new Set<string>();
    this.allUnits.forEach(unit => {
      if (unit.villageName) {
        villageSet.add(unit.villageName);
      }
    });
    this.villages = Array.from(villageSet).sort();
  }

  applyFilters(): void {
    if (this.allUnits.length === 0) return;

    let filtered = [...this.allUnits];

    // Filter by village
    if (this.selectedVillage) {
      filtered = filtered.filter(unit =>
        unit.villageName === this.selectedVillage
      );
    }

    // Filter by unit type
    if (this.selectedType) {
      const typeMap: { [key: string]: number } = {
        'Apartment': 0,
        'Villa': 1,
        'Chalet': 2
      };
      const selectedTypeNumber = typeMap[this.selectedType];
      if (selectedTypeNumber !== undefined) {
        filtered = filtered.filter(unit => unit.unitType === selectedTypeNumber);
      }
    }

    // Filter by bedrooms
    if (this.selectedBedrooms) {
      if (this.selectedBedrooms === '4+') {
        filtered = filtered.filter(unit => (unit.bedrooms ?? 0) >= 4);
      } else {
        const bedrooms = parseInt(this.selectedBedrooms);
        filtered = filtered.filter(unit => (unit.bedrooms ?? 0) === bedrooms);
      }
    }

    // Filter by bathrooms
    if (this.selectedBathrooms) {
      if (this.selectedBathrooms === '3+') {
        filtered = filtered.filter(unit => (unit.bathrooms ?? 0) >= 3);
      } else {
        const bathrooms = parseInt(this.selectedBathrooms);
        filtered = filtered.filter(unit => (unit.bathrooms ?? 0) === bathrooms);
      }
    }

    // Filter by price range
    if (this.minPrice !== null && this.minPrice !== undefined) {
      filtered = filtered.filter(unit => (unit.basePricePerNight ?? 0) >= this.minPrice!);
    }

    if (this.maxPrice !== null && this.maxPrice !== undefined) {
      filtered = filtered.filter(unit => (unit.basePricePerNight ?? 0) <= this.maxPrice!);
    }

    this.filteredUnits = filtered;
    this.updatePaginatedUnits();
    this.cdr.detectChanges();
  }

  updatePaginatedUnits(): void {
    // Get only the latest units up to maxUnitsToShow
    const limitedUnits = this.filteredUnits.slice(-this.maxUnitsToShow);
    this.totalPropertyPages = Math.ceil(limitedUnits.length / this.propertiesPerPage);

    // Reset to first page if current page is out of bounds
    if (this.currentPropertyPage >= this.totalPropertyPages) {
      this.currentPropertyPage = 0;
    }

    const startIndex = this.currentPropertyPage * this.propertiesPerPage;
    const endIndex = startIndex + this.propertiesPerPage;
    this.paginatedUnits = limitedUnits.slice(startIndex, endIndex);
  }

  getUnitImagePath(unit: Unit): string {
    return unit.imageURL ?? '';
  }

  // Slider methods
  startSlider(): void {
    if (this.isBrowser) {
      this.stopSlider();
      this.slideInterval = setInterval(() => {
        this.currentSlide = (this.currentSlide + 1) % this.slides.length;
      }, 5000);
    }
  }

  stopSlider(): void {
    if (this.isBrowser && this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
    if (this.isBrowser) {
      this.startSlider();
    }
  }

  // Properties carousel methods
  goToPropertyPage(pageIndex: number): void {
    if (pageIndex >= 0 && pageIndex < this.totalPropertyPages) {
      this.currentPropertyPage = pageIndex;
      this.updatePaginatedUnits();
      this.cdr.detectChanges();
    }
  }

  nextPropertyPage(): void {
    if (this.currentPropertyPage < this.totalPropertyPages - 1) {
      this.currentPropertyPage++;
      this.updatePaginatedUnits();
      this.cdr.detectChanges();
    }
  }

  previousPropertyPage(): void {
    if (this.currentPropertyPage > 0) {
      this.currentPropertyPage--;
      this.updatePaginatedUnits();
      this.cdr.detectChanges();
    }
  }

  // Search form methods
  toggleMoreOptions(event: Event): void {
    event.preventDefault();
    this.showMoreOptions = !this.showMoreOptions;
  }

  applySearchFilters(): void {
    // Validate price range
    this.validatePriceRange();

    // Set searching state
    this.isSearching = true;
    this.searchPerformed = true;

    // Update search service
    this.searchService.updateSearchCriteria({
      selectedVillage: this.selectedVillage,
      selectedType: this.selectedType,
      selectedBedrooms: this.selectedBedrooms,
      selectedBathrooms: this.selectedBathrooms,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice
    });

    // Simulate search delay for better UX
    setTimeout(() => {
      // Apply filters immediately
      this.applyFilters();
      this.isSearching = false;
      this.cdr.detectChanges();

      // Navigate to units page with search parameters
      this.navigateToUnitsPage();
    }, 500);
  }

  navigateToUnitsPage(): void {
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
    if (this.minPrice) {
      queryParams['minPrice'] = this.minPrice.toString();
    }
    if (this.maxPrice) {
      queryParams['maxPrice'] = this.maxPrice.toString();
    }

    this.router.navigate(['/units'], { queryParams });
  }

  validatePriceRange(): void {
    if (this.minPrice && this.maxPrice && this.minPrice > this.maxPrice) {
      const temp = this.minPrice;
      this.minPrice = this.maxPrice;
      this.maxPrice = temp;
    }
  }

  // Helper methods for search
  hasActiveFilters(): boolean {
    return !!(
      this.selectedVillage ||
      this.selectedType ||
      this.selectedBedrooms ||
      this.selectedBathrooms ||
      this.minPrice ||
      this.maxPrice
    );
  }

  clearFilters(): void {
    this.selectedVillage = null;
    this.selectedType = null;
    this.selectedBedrooms = null;
    this.selectedBathrooms = null;
    this.minPrice = null;
    this.maxPrice = null;
    this.showMoreOptions = false;
    this.searchPerformed = false;

    this.searchService.clearSearchCriteria();
    this.applyFilters();
  }

  getFilteredUnitsCount(): number {
    return this.filteredUnits.length;
  }

  getTotalUnitsCount(): number {
    return this.allUnits.length;
  }

  // Enhanced search validation
  isSearchValid(): boolean {
    // Check if at least one filter is applied
    if (!this.hasActiveFilters()) {
      return false;
    }

    // Validate price range
    if (this.minPrice && this.maxPrice && this.minPrice > this.maxPrice) {
      return false;
    }

    // Validate numeric inputs
    if (this.minPrice && this.minPrice < 0) {
      return false;
    }

    if (this.maxPrice && this.maxPrice < 0) {
      return false;
    }

    return true;
  }

  // Get search summary for display
    getSearchSummary(): string {
    const filters = [];

    if (this.selectedVillage) {
      filters.push(`Location: ${this.selectedVillage}`);
    }
    if (this.selectedType) {
      filters.push(`Type: ${this.selectedType}`);
    }
    if (this.selectedBedrooms) {
      filters.push(`Bedrooms: ${this.selectedBedrooms}`);
    }
    if (this.selectedBathrooms) {
      filters.push(`Bathrooms: ${this.selectedBathrooms}`);
    }
    if (this.minPrice || this.maxPrice) {
      const priceRange = [];
      if (this.minPrice) priceRange.push(`Min: $${this.minPrice}`);
      if (this.maxPrice) priceRange.push(`Max: $${this.maxPrice}`);
      filters.push(`Price: ${priceRange.join(' - ')}`);
    }

    return filters.join(', ');
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img) {
      img.src = 'assets/images/default-unit.jpg';
    }
  }
}
