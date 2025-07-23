import { QrCodeDto } from './../Models/qr-code-dto';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams, httpResource } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { Observable } from 'rxjs';
import { ReviewDTO } from '../Models/ReviewDTO';
import { UrlCodec } from '@angular/common/upgrade';

@Injectable({
  providedIn: 'root'
})
export class QrServise {
  QrURL =   "https://localhost:7083/api/QrCode"
constructor(  private http :HttpClient,@Inject(PLATFORM_ID) private platformId: Object){}

  get token(){
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem("token")?.toString();
    }
    return null;
  }
  private get headers(){
    return new HttpHeaders({
      // "Content-Type":"Content-Type:application/json",
      Authorization: "Bearer "+ this.token
    })
  }

  createQr():Observable<any>{
    let Qrdto:QrCodeDto={
      BookingId : 1,
      // expirationDate : "",
      TenantNationalId : "2341",
      VillageName : "Mousa Cost",
      UnitAddress : "Alex",
      OwnerName : "mark  Owner",
      TenantName : "tenant name",
      
    };
    console.log(`${this.QrURL}/create`)
    return this.http.post<any>(`${this.QrURL}/create`,Qrdto, {headers:this.headers});
  }
}
