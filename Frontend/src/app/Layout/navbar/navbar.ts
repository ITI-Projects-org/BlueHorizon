import {
  Component,
  HostListener,
  ViewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements AfterViewInit, OnDestroy {
  @ViewChild('mainNavbar') mainNavbar!: ElementRef;
  @ViewChild('heroSection') heroSection!: ElementRef;

  currentSlide = 0;
  private slideInterval: any;
  showMoreOptions = false;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  isScrolled = false;

  // Slider images array
  slides = [
    { image: 'imges/1.jpg', alt: 'Modern Villa' },
    { image: 'imges/3.jpg', alt: 'Luxury Apartment' },
    {
      image: 'imges/photo-1564013799919-ab600027ffc6.jpeg',
      alt: 'Beautiful House',
    },
  ];

  @HostListener('window:scroll')
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
    this.updateNavbarStyle();
  }

  @HostListener('window:resize')
  onWindowResize() {
    this.adjustHeroPadding();
  }

  ngAfterViewInit() {
    this.adjustHeroPadding();
    this.startSlider();
    this.updateNavbarStyle();
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
    // Reset timer when manually changing slides
    clearInterval(this.slideInterval);
    this.slideInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.slides.length;
    }, 5000);
  }

  toggleMoreOptions(event: Event) {
    event.preventDefault();
    this.showMoreOptions = !this.showMoreOptions;
  }

  validatePriceRange() {
    if (
      this.minPrice !== null &&
      this.maxPrice !== null &&
      this.minPrice > this.maxPrice
    ) {
      alert('Minimum price cannot be greater than maximum price');
      this.maxPrice = null;
    }
  }
}
