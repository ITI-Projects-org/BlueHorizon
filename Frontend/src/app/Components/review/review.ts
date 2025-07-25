import { ReviewDTO } from './../../Models/ReviewDTO';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ReviewService } from '../../Services/review-service';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common'; // Import DatePipe


@Component({
  selector: 'app-review',
  imports: [FormsModule, DatePipe],
  templateUrl: './review.html',
  standalone: true,
  styleUrl: './review.css',
})
export class Review implements OnInit {
  constructor(private reviewService: ReviewService, private cdr:ChangeDetectorRef) {}
  ngOnInit(): void {}

  rating:number=0;
  comment:string="";
  reviewDTO!: ReviewDTO;

  SubmitReview() {
    this.reviewDTO = {
      unitId: 1,
      bookingId: 2,
      rating: this.rating,
      comment: this.comment,
      reviewStatus: 0,
      tenantName:null,
       reviewDate:null
    };
    this.reviewService.AddReview(this.reviewDTO).subscribe({
      next: (r) => console.log(r),
      error: (e) => console.log(e),
    });
  }
  deleteReview(reviewId: number) {
    console.log('btn clicked');
    this.reviewService.DeleteReview(reviewId).subscribe({
      next: (r) => console.log(r),
      error: (r) => console.log(r),
    });
  }
  allReviews: ReviewDTO[] = [];
  getAllReviews(untiId: number) {
    this.reviewService.getAllReviews(untiId).subscribe({
      next: (res) => {console.log(res);
        this.allReviews = res as unknown as ReviewDTO[] ;
        this.cdr.detectChanges();
      },
      error: (e) => console.log(e),
      
    });
  }
}
