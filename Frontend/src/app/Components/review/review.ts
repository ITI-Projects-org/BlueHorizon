import { ReviewDTO } from './../../Models/ReviewDTO';
import { Component, OnInit } from '@angular/core';
import { ReviewService } from '../../Services/review-service';
import { serverRoutes } from '../../app.routes.server';

@Component({
  selector: 'app-review',
  imports: [],
  templateUrl: './review.html',
  styleUrl: './review.css'
})
export class Review implements OnInit {
  constructor(private reviewService:ReviewService){}
  ngOnInit(): void {
    // this.SubmitReview();
  }
  
  reviewDTO!: ReviewDTO;
  SubmitReview(){
    this.reviewDTO = {
      unitId: 1,
      bookingId: 1,
      rating: 3,
      comment: "from angular",
      reviewStatus: 0
    }
    this.reviewService.AddReview(this.reviewDTO).subscribe({
      next: r => console.log(r),
      error: e => console.log(e)
    });
  }
  deleteReview(reviewId:number){
    console.log('btn clicked')
    this.reviewService.DeleteReview(reviewId).subscribe({
      next:r=>console.log(r),
      error:r=>console.log(r)
    });  
  }
  getAllReviews(untiId:number){
    this.reviewService.getAllReviews(untiId).subscribe({
      next:e=>console.log(e),
      error:e=>console.log(e)
    })
  }
    
}
