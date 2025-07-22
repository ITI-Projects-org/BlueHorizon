import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, httpResource } from '@angular/common/http';
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

  AddReview(reviewDTO: ReviewDTO):Observable<any>{
    return this.http.post<any>(this.reviewURL,reviewDTO,{headers:this.headers});
  }
  private get headers(){
    return new HttpHeaders({
      // "Content-Type":"Content-Type:application/json",
      Authorization: "Bearer "+ this.token
    })
  }
}
