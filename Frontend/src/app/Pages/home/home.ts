import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
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
      price: 120000,
    },
    {
      id: 2,
      image: 'assets/default-unit.jpg',
      title: 'Luxury Villa in Giza',
      address: '6th of October, Giza',
      bedrooms: 4,
      bathrooms: 3,
      price: 350000,
    },
    {
      id: 3,
      image: 'assets/default-unit.jpg',
      title: 'Chalet by the Sea',
      address: 'North Coast',
      bedrooms: 3,
      bathrooms: 2,
      price: 200000,
    },
  ];
}
