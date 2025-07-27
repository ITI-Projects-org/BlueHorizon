// app/Pages/home/home.ts
import { ChangeDetectorRef, Component, OnInit, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { ActivatedRoute, InitialNavigation, RouterLink, Router } from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { SearchService } from '../../Services/searchService';
import { Unit } from '../../Models/unit.model';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  // Hero Section - Slider properties
  slides = [
    { image: 'images/1.jpg' }, // Corrected path to 'imges/'
    { image: 'images/3.jpg' },
    { image: 'images/2.jpeg' } // Corrected path to 'imges/'
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
      private route: ActivatedRoute,
      private searchService: SearchService,
      private cdr: ChangeDetectorRef
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

    filteredUnits: Unit[] = [];
    paginatedUnits: Unit[] = [];

    isLoading = true;
    error: string | null = null;

    ngOnInit(): void {
        this.fetchUnits();
    if (this.isBrowser) {
      this.startSlider();
    }

    this.activatedRoute.queryParams.subscribe(params => {
      this.selectedVillage = params['village'] || null;
      this.selectedType = params['type'] || null;
      this.selectedBedrooms = params['bedrooms'] || null;
      this.selectedBathrooms = params['bathrooms'] || null;
      this.minPrice = params['minPrice'] ? parseFloat(params['minPrice']) : null;
      this.maxPrice = params['maxPrice'] ? parseFloat(params['maxPrice']) : null;

      if (this.selectedBedrooms || this.selectedBathrooms || this.minPrice || this.maxPrice) {
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

                this.paginatedUnits = data.slice(-6);
                console.log(this.paginatedUnits)
                this.cdr.detectChanges()
            },
            error: (err) => {
                console.error('Error fetching units', err);
                this.error = 'Failed to load units. Please try again later.';
                this.isLoading = false;
            },
        });
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

    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: queryParams,
      queryParamsHandling: 'merge'
    });

    console.log('Applying filters:', queryParams);
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
