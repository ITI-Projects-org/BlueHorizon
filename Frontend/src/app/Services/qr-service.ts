import { QrCodeDto } from './../Models/qr-code-dto';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { Observable, catchError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class QrServise {
  QrURL = 'https://localhost:7083/api/QrCode';
  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  get token() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('accessToken')?.toString();
    }
    return null;
  }
  private get headers() {
    return new HttpHeaders({
      // "Content-Type":"Content-Type:application/json",
      Authorization: 'Bearer ' + this.token,
    });
  }

  createQr(): Observable<{ message: string; qrId: number }> {
    console.log('from post in service');
    let Qrdto: QrCodeDto = {
      BookingId: 2,
      TenantNationalId: '2341',
      VillageName: 'Mousa Cosst',
      UnitAddress: 'Alex',
      OwnerName: 'mark Owner',
      TenantName: 'tenant name',
    };
    console.log(`${this.QrURL}/create`);
    return this.http.post<{ message: string; qrId: number }>(
      `${this.QrURL}/create`,
      Qrdto,
      { headers: this.headers }
    );
  }

  getQrCode(qrId: number): Observable<Blob> {
    console.log('from get in service');

    return this.http.get(`${this.QrURL}/${qrId}`, {
      headers: this.headers,
      responseType: 'blob',
    });
  }

  // -------------------------
  // -------------------------
  // -------------------------
  // -------------------------
  // ----------CLOUD----------
  // -------------------------
  // -------------------------
  // -------------------------

  createQrCloud(qrData: QrCodeDto): Observable<{
    message: string;
    qrId: number;
    imgPath: string;
  }> {
    // Validate required fields
    if (!qrData.BookingId || !qrData.TenantNationalId) {
      throw new Error('BookingId and TenantNationalId are required fields');
    }

    return this.http
      .post<{ message: string; qrId: number; imgPath: string }>(
        `${this.QrURL}/createCloud`,
        qrData,
        {
          headers: this.headers,
        }
      )
      .pipe(
        catchError((error: any) => {
          let errorMessage = 'An error occurred while creating the QR code';
          if (error.error?.message) {
            errorMessage = error.error.message;
          } else if (error.status === 401) {
            errorMessage = 'You are not authorized to create QR codes';
          }
          throw new Error(errorMessage);
        })
      );
  }
  getQrCodeCloud(qrId: number): Observable<{ imgPath: string }> {
    return this.http.get<{ imgPath: string }>(`${this.QrURL}/Cloud/${qrId}`, {
      headers: this.headers,
    });
  }
}
