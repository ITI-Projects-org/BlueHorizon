import { CommonModule, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Verification } from '../../../Services/verification-service';
import { OwnerVerification } from '../owner-verification/owner-verification';
import { OwnerVerificationDTO } from '../../../Models/owner-verification';
import { RespondVerificationDTO } from '../../../Models/respond-verification-dto';

@Component({
  selector: 'app-pending-owners',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe],
  templateUrl: './pending-owners.html',
  styleUrl: './pending-owners.css'
})
export class PendingOwners implements OnInit{
  requests!:any;
  RespondVerificationDTO!:RespondVerificationDTO;
  constructor(private VerificationService:Verification, private cdr:ChangeDetectorRef){}
  ngOnInit(): void {
    this.loadRequests();
  }
  loadRequests(){
    this.VerificationService.GetPendingOwners().subscribe({
      next:(res)=>{
        // let ress = res as any; 
        console.log(typeof res);
        console.log("res: ");
        console.log(res);
        let index;
        index = Object.keys(res).findIndex(r=>r=="result");
        console.log(Object.values(res)[index]);
        this.requests = Object.values(res)[index];
        this.requests =res;
        
        // console.log(Object.values(res));
        this.cdr.detectChanges(); 
      }
    });
  }
  Respond(unitId:number, verificationStatus:number, ownerId:string){
    this.RespondVerificationDTO={ UnitId:unitId, verificationStatus, ownerId }
    this.VerificationService.RespondToVerificationRequest(this.RespondVerificationDTO).subscribe({
    
    next:(res)=>{
      this.loadRequests()
    }
    })
  }

  trackById(index: number, item: any) {
    return item.id;
  }
}
