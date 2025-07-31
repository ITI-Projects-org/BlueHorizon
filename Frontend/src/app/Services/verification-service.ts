import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OwnerVerificationDTO } from './../Models/owner-verification';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Observable } from 'rxjs';
import { RespondVerificationDTO } from '../Models/respond-verification-dto';
import { isPlatformBrowser } from '@angular/common';
import { UnitVerificationDTO } from '../Models/unit-verification-dto';

@Injectable({
  providedIn: 'root',
})
export class Verification {
  private verificationURL = 'https://localhost:7083/api/Verification';
  private AmenityURL = 'https://localhost:7083/api/Amenity';

  private ownerVerificationDTO!: OwnerVerificationDTO;
  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // VerifiyOwner(data: FormData) {
  //   const headers = new HttpHeaders().set(
  //     'Authorization',
  //     'Bearer ' + localStorage.getItem('accessToken')
  //   );
  //   // Tell HttpClient to expect a plain text response to avoid JSON parsing errors.
  //   return this.http.post(this.verificationURL + '/AddRequest', data, {
  //     headers: headers,
  //     responseType: 'text',
  //   });
  // }

  VerifiyOwner(data: FormData) {
    //const headers = new HttpHeaders().set(
    //  'Authorization',
    //  'Bearer ' + localStorage.getItem('accessToken')
    //);
    // Tell HttpClient to expect a plain text response to avoid JSON parsing errors.
      return this.http.post(this.verificationURL + '/AddRequest', data);
      //{
      //    headers: headers,
      //        responseType: 'text',
      //}
  }

  isVerified(): Observable<{ isVerified: boolean }> {
    console.log('from is verifed serivce');
    console.log(`${this.verificationURL}/isVerified`);
    return this.http.get<{ isVerified: boolean }>(
      `${this.verificationURL}/isVerified`,
      { headers: this.headers }
    );
  }

  GetPendingOwners(): Observable<OwnerVerificationDTO[]> {
    return this.http.get<OwnerVerificationDTO[]>(
      `${this.verificationURL}/Requests`,
      { headers: this.headers }
    );
  }
  RespondToVerificationRequest(
    ResoondVerificationDTO: RespondVerificationDTO
  ): Observable<any> {
    return this.http.post(
      `${this.verificationURL}/Respond`,
      ResoondVerificationDTO,
      { headers: this.headers }
    );
  }

  GetAllAmenities(): Observable<{ id: number; name: string }> {
    return this.http.get<{ id: number; name: string }>(`${this.AmenityURL}`);
  }
  private get headers() {
    return new HttpHeaders({
      // "Content-Type":"multipart/form-data",
      Authorization: 'Bearer ' + this.token,
    });
  }
  get token() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('token')?.toString();
    }
    return null;
  }


// ___________________________________
// ___________________________________
// _____________For Units_____________
// ___________________________________
// ___________________________________


  GetPendingUnits(): Observable<UnitVerificationDTO[]> {
    return this.http.get<UnitVerificationDTO[]>(
      `${this.verificationURL}/UnitRequests`,
      { headers: this.headers }
    );
  }




  RespondUnitVerificationDTO(
    ResoondVerificationDTO: RespondVerificationDTO
  ): Observable<any> {
    return this.http.post(
      `${this.verificationURL}/UnitRespond`,
      ResoondVerificationDTO,
      { headers: this.headers }
    );
  }
}
