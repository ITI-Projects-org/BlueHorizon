// app/Pages/home/home.ts
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, InitialNavigation, RouterLink } from '@angular/router';
import { UnitsService } from '../../Services/units.service';
import { SearchService } from '../../Services/searchService';
import { Unit } from '../../Models/unit.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit{
  
  constructor(
    private unitsService: UnitsService,
    private route: ActivatedRoute,
    private searchService: SearchService,
    private cdr:ChangeDetectorRef
  ) {}
  // allUnits: Unit[] = [];
    filteredUnits: Unit[] = [];
    paginatedUnits: Unit[] = [];
  
    isLoading = true;
    error: string | null = null;
    ngOnInit(): void {
      this.fetchUnits();
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
  
  
}
