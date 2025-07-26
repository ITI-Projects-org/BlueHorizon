import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Unit } from '../../Services/unit';
import { IUnit } from '../../Models/iunit';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-unit-details',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatIconModule],
  templateUrl: './unit-details.html',
  styleUrl: './unit-details.css'
})
export class UnitDetailsComponent implements OnInit {
  unit: IUnit | null = null;
  loading = true;
  error: string | null = null;

  constructor(private route: ActivatedRoute, private unitService: Unit) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error = 'Invalid unit ID.';
      this.loading = false;
      return;
    }
    this.unitService.GetUnitById(id).pipe(
      catchError(err => {
        this.error = 'Failed to load unit details.';
        this.loading = false;
        return of(null);
      })
    ).subscribe((unit) => {
      this.unit = unit;
      this.loading = false;
    });
  }
}