import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DecimalPipe, NgForOf } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [DecimalPipe, RouterLink, NgForOf],
  standalone: true,
  templateUrl: './home.html',
})
export class Home {
  public latestProperties = [
    {
      id: 1,
      image: 'assets/default-unit.jpg',
      title: 'Modern Apartment in Cairo',
      address: 'Zamalek, Cairo',
      bedrooms: 2,
      bathrooms: 2,
      price: 120000
    },
    {
      id: 2,
      image: 'assets/default-unit.jpg',
      title: 'Luxury Villa in Giza',
      address: '6th of October, Giza',
      bedrooms: 4,
      bathrooms: 3,
      price: 350000
    },
    {
      id: 3,
      image: 'assets/default-unit.jpg',
      title: 'Chalet by the Sea',
      address: 'North Coast',
      bedrooms: 3,
      bathrooms: 2,
      price: 200000
    }
  ];
} 