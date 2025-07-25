import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Unit } from '../Models/unit.model';

@Injectable({
  providedIn: 'root'
})
export class UnitsService {
  private apiUrl = 'https://localhost:7083/api/unit/all';

  constructor(private http: HttpClient) {}

  getUnits(): Observable<Unit[]> {
    return this.http.get<Unit[]>(this.apiUrl);
  }
}