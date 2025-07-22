import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Iunit } from '../models/iunit';

@Injectable({
  providedIn: 'root'
})
export class Unit {
  baseurl: string = 'https://localhost:7083/api/Unit';
  constructor(private http: HttpClient) {
    // Initialization logic for the Unit service
  }
  AddUnit(formData: FormData):Observable<Iunit> {
    return this.http.post<Iunit>(`${this.baseurl}/AddUnit`,formData);
  }
  VerifyUnit(id:number):Observable<any>{
    return this.http.get(`${this.baseurl}/VerifyUnit/${id}`);
  }
  GetUnitById(id: number): Observable<Iunit> {
    return this.http.get<Iunit>(`${this.baseurl}/GetUnitById/${id}`);
  }
  UpdateUnit(id: number, unit: Iunit): Observable<Iunit> {
    return this.http.put<Iunit>(`${this.baseurl}/UpdateUnit/${id}`, unit);
  }
  DeleteUnit(id: number): Observable<any> {
    return this.http.delete(`${this.baseurl}/DeleteUnit/${id}`);
  }


}
