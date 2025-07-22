import { ReviewDTO } from './../../Models/ReviewDTO';
import { Component, OnInit } from '@angular/core';
import { ReviewService } from '../../Services/review-service';

@Component({
  selector: 'app-review',
  imports: [],
  templateUrl: './review.html',
  styleUrl: './review.css'
})
export class Review implements OnInit {
  constructor(private reviewService:ReviewService){}
  ngOnInit(): void {
    this.SubmitReview();
  }
  
  reviewDTO!: ReviewDTO;
  SubmitReview(){
    this.reviewDTO = {
      unitId: 20,
      bookingId: 2,
      rating: 5,
      comment: "good review",
      reviewStatus: 0
    }
    this.reviewService.AddReview(this.reviewDTO).subscribe({
      next:r=>console.log(r)
    });

    
  }
}
