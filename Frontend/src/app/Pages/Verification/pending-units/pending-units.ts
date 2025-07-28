import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { constructUserAgent } from '@microsoft/signalr/dist/esm/Utils';
import { Verification } from '../../../Services/verification-service';
import { UnitVerificationDTO } from '../../../Models/unit-verification-dto';
import { createDecipheriv } from 'crypto';

@Component({
  selector: 'app-pending-units',
  imports: [],
  templateUrl: './pending-units.html',
  styleUrl: './pending-units.css'
})
export class PendingUnits implements OnInit, OnDestroy{
  constructor(private verificationService: Verification, private cdr:ChangeDetectorRef) {}
  ngOnDestroy(): void {
   this.sub.unsubscribe();
  }

  ngOnInit(): void {
    
    this.loadPendingUnits();
  }
  sub!:any;
  pendingUnits : UnitVerificationDTO[] = [];
  loadPendingUnits(){
    this.sub = this.verificationService.GetPendingUnits().subscribe({
      next: (units) => {
        this.pendingUnits = units;
        // console.log(units)  
        console.log(this.pendingUnits)  
        this.cdr.detectChanges();
      },
    })
  }
}
