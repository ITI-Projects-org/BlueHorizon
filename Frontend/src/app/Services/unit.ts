import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IUnit } from '../Models/iunit';

@Injectable({
  providedIn: 'root',
})
export class Unit {
  baseurl: string = 'https://localhost:7083/api/Unit';
  constructor(private http: HttpClient) {}
  AddUnit(formData: FormData): Observable<IUnit> {
    return this.http.post<IUnit>(`${this.baseurl}/AddUnit`, formData);
  }
  VerifyUnit(id: number): Observable<any> {
    return this.http.get(`${this.baseurl}/VerifyUnit/${id}`);
  }
  GetUnitById(id: number): Observable<IUnit> {
    return this.http.get<IUnit>(`${this.baseurl}/GetUnitById/${id}`);
  }
  UpdateUnit(id: number, unit: IUnit): Observable<IUnit> {
    return this.http.put<IUnit>(`${this.baseurl}/UpdateUnit/${id}`, unit);
  }
  DeleteUnit(id: number): Observable<any> {
    return this.http.delete(`${this.baseurl}/DeleteUnit/${id}`);
  }
  GetMyUnits(): Observable<IUnit[]> {
    return this.http.get<IUnit[]>(`${this.baseurl}/MyUnits`);
  }
}
