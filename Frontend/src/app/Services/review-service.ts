import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, HttpParams, httpResource } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { Observable } from 'rxjs';
import { ReviewDTO } from '../Models/ReviewDTO';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
reviewURL =   "https://localhost:7083/api/Review"
constructor(  private http :HttpClient,@Inject(PLATFORM_ID) private platformId: Object){}

get token(){
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem("token")?.toString();
    }
    return null;
  }
  
  //     const params = new HttpParams().set('unitId', unitId.toString());


  AddReview(reviewDTO: ReviewDTO):Observable<any>{
    return this.http.post<any>(this.reviewURL,reviewDTO,{headers:this.headers});
  }
  DeleteReview(reviewId:number):Observable<any>{
    console.log(`endpoint hit: `);
    console.log(`${this.reviewURL}/${reviewId}`);
    return this.http.delete<any>(`${this.reviewURL}/${reviewId}`,{headers:this.headers});
    
  }

  getAllReviews(unitId:number):Observable<{ reviewDate:Date|null,unitId:number,bookingId :number,rating:number,comment :string|null,reviewStatus : number,tenantName:string|null}>{
    console.log('getting all reviews')
    console.log('of unit : '+unitId)
    // const params = new HttpParams().set("unitId",unitId)
    console.log(`${this.reviewURL}/GetAllUnitReviews/${unitId}`);

    return this.http.get<{ reviewDate:Date|null,unitId:number,bookingId :number,rating:number,comment :string|null,reviewStatus : number,tenantName:string|null}>(`${this.reviewURL}/GetAllUnitReviews/${unitId}`,{headers:this.headers});
  }
  private get headers(){
    return new HttpHeaders({
      // "Content-Type":"Content-Type:application/json",
      Authorization: "Bearer "+ this.token
    })
  }
}
