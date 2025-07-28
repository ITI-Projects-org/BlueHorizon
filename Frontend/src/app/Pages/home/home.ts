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
    { image: 'images/1.jpg' }, // Corrected path to 'imges/'
    { image: 'images/3.jpg' },
    { image: 'images/2.jpeg' }, // Corrected path to 'imges/'
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

  // Dummy data for dropdowns (replace with actual data from a service if needed)
  villages: string[] = ['Village A', 'Village B', 'Village C'];
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
  error: string | null = null;

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
    });
  }

  ngOnDestroy(): void {
    if (this.isBrowser) {
      this.stopSlider();
    }
  }

  fetchUnits(): void {
    this.isLoading = true;
    this.unitsService.getUnits().subscribe({
      next: (data) => {
        this.isLoading = false;
        // Get only the latest 9 units
        this.allUnits = data.slice(-this.maxUnitsToShow);
        this.totalPropertyPages = Math.ceil(
          this.allUnits.length / this.propertiesPerPage
        );
        this.updatePaginatedUnits();
        console.log(this.paginatedUnits);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching units', err);
        this.error = 'Failed to load units. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  updatePaginatedUnits(): void {
    const startIndex = this.currentPropertyPage * this.propertiesPerPage;
    const endIndex = startIndex + this.propertiesPerPage;
    this.paginatedUnits = this.allUnits.slice(startIndex, endIndex);
  }
  getUnitImagePath(unit: Unit): string {
    // console.log('this is image paths');
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

    // Navigate to units page with the selected filters
    this.router.navigate(['/units'], {
      queryParams: queryParams,
    });

    console.log('Navigating to units page with filters:', queryParams);
  }

  validatePriceRange() {
    if (
      this.minPrice !== null &&
      this.maxPrice !== null &&
      this.minPrice > this.maxPrice
    ) {
      console.error('Minimum price cannot be greater than maximum price');
      this.maxPrice = null;
    }
  }
}
