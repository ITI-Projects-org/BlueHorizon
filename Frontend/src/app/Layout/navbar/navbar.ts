// src/app/Layout/navbar/navbar.ts
import { Component, HostListener, ViewChild, ElementRef, AfterViewInit, OnDestroy, OnInit } from '@angular/core';
declare var bootstrap: any;

import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SearchService, SearchCriteria } from './../../services/searchService';
import { UnitsService } from '../../services/units.service';
import { Unit } from '../../Models/unit.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class Navbar implements AfterViewInit, OnDestroy, OnInit {
  @ViewChild('mainNavbar') mainNavbar!: ElementRef;
  @ViewChild('heroSection') heroSection!: ElementRef;

  currentSlide = 0;
  private slideInterval: any;
  showMoreOptions = false;
  isScrolled = false;

  selectedBedrooms?: string | null = null;
  selectedBathrooms?: string | null = null;
  minPrice?: number | null = null;
  maxPrice?: number | null = null;
  selectedVillage?: string | null = null;
  selectedType?: string | null = null;

  villages: (string | undefined)[] = [];
  unitTypes: string[] = [];

  slides = [
    { image: 'imges/1.jpg', alt: 'Modern Villa' },
    { image: 'imges/3.jpg', alt: 'Luxury Apartment' },
    { image: 'imges/photo-1564013799919-ab600027ffc6.jpeg', alt: 'Beautiful House' }
  ];

  constructor(
    private searchService: SearchService,
    private router: Router,
    private unitsService: UnitsService
  ) {}

  ngOnInit(): void {
    this.searchService.searchCriteria$.subscribe(criteria => {
      this.selectedBedrooms = criteria.selectedBedrooms;
      this.selectedBathrooms = criteria.selectedBathrooms;
      this.minPrice = criteria.minPrice;
      this.maxPrice = criteria.maxPrice;
      this.selectedVillage = criteria.selectedVillage;
      this.selectedType = criteria.selectedType;
    });

    this.unitsService.getUnits().subscribe(units => {
       console.log('Loaded units:', units);
         console.log('First unit sample:', units[0]);
      this.villages = Array.from(
        new Set(units.map(u => u.villageName).filter(v => v))
      );

      const unitTypeMap: { [key: number]: string } = {
        0: 'Apartment',
        1: 'Villa',
        2: 'Chalet'
      };

      this.unitTypes = Array.from(
        new Set(units.map(u => unitTypeMap[u.unitType!]).filter(t => t))
      );
      
  console.log('Villages:', this.villages);   
  console.log('Types:', this.unitTypes);    
    });
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
    this.updateNavbarStyle();
  }

  ngAfterViewInit() {
    this.adjustHeroPadding();
    this.startSlider();
    this.updateNavbarStyle();
     const dropdownElements = document.querySelectorAll('.dropdown-toggle');
    dropdownElements.forEach(dropdown => {
      new bootstrap.Dropdown(dropdown);
    });
  }

  ngOnDestroy() {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  private updateNavbarStyle() {
    if (this.mainNavbar) {
      const navbar = this.mainNavbar.nativeElement;
      if (this.isScrolled) {
        navbar.classList.add('scrolled');
      } else {
        navbar.classList.remove('scrolled');
      }
    }
  }

  private adjustHeroPadding() {
    if (this.mainNavbar && this.heroSection) {
      const navbarHeight = this.mainNavbar.nativeElement.offsetHeight;
      this.heroSection.nativeElement.style.paddingTop = `${navbarHeight}px`;
    }
  }

  private startSlider() {
    this.slideInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.slides.length;
    }, 5000);
  }

  goToSlide(index: number) {
    this.currentSlide = index;
    clearInterval(this.slideInterval);
    this.slideInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.slides.length;
    }, 5000);
  }

  toggleMoreOptions(event: Event) {
    event.preventDefault();
    this.showMoreOptions = !this.showMoreOptions;
  }

  applySearchFilters() {
    const currentCriteria: SearchCriteria = {
      selectedBedrooms: this.selectedBedrooms,
      selectedBathrooms: this.selectedBathrooms,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice,
      selectedVillage: this.selectedVillage,
      selectedType: this.selectedType
    };
    this.searchService.updateSearchCriteria(currentCriteria);
    this.router.navigate(['/units']);
  }
} 
