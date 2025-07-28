import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { constructUserAgent } from '@microsoft/signalr/dist/esm/Utils';
import { Verification } from '../../../Services/verification-service';
import { UnitVerificationDTO } from '../../../Models/unit-verification-dto';
import { createDecipheriv } from 'crypto';
import { RespondVerificationDTO } from '../../../Models/respond-verification-dto';
import { CommonModule} from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-pending-units',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pending-units.html',
  styleUrl: './pending-units.css'
})

export class PendingUnits implements OnInit, OnDestroy{
  constructor(private VerificationService: Verification, private cdr:ChangeDetectorRef) {}
  ngOnDestroy(): void {
   this.sub.unsubscribe();
  }

  ngOnInit(): void {
    
    this.loadRequests();
  }
    RespondVerificationDTO!: RespondVerificationDTO;
  
  sub!:any;
  requests : UnitVerificationDTO[] = [];
  loadRequests(){
    this.sub = this.VerificationService.GetPendingUnits().subscribe({
      next: (units) => {
        this.requests = units;
        console.log(this.requests)  
        this.cdr.detectChanges();
      },
    })
  } 
  Respond(unitId: number, verificationStatus: number) {
    this.RespondVerificationDTO = {
      UnitId: unitId,
      verificationStatus,
      
    };
    this.VerificationService.RespondUnitVerificationDTO(
      this.RespondVerificationDTO
    ).subscribe({
      next: (res) => {
        this.loadRequests();
      },
    });
  }
    trackById(index: number, item: any) {
    return item.id;
  }
}
