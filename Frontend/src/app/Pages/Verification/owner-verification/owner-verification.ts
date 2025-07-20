import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Verification } from '../../../Services/verification-service';
import { OwnerVerificationDTO } from '../../../Models/owner-verification';


@Component({
  selector: 'app-owner-verification',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './owner-verification.html',
  styleUrl: './owner-verification.css'
})
export class OwnerVerification {
  constructor(private OwnerVerificationService:Verification) {}
  ownerDTO!:OwnerVerificationDTO;

  ownerForm = new FormGroup({
    DocumentType :new FormControl(0,[Validators.required]),
    DocumentPath :new FormControl(''),
    NationalId :new FormControl('',[Validators.required]),
    BankAccountDetails :new FormControl('',[Validators.required]),
    Title :new FormControl('',[Validators.required]),
    Description :new FormControl(''),
    UnitType :new FormControl(0),
    Bedrooms :new FormControl(0),
    Bathrooms :new FormControl(0),
    Sleeps :new FormControl(0),
    DistanceToSea :new FormControl(0),
    BasePricePerNight :new FormControl(0),
    Address :new FormControl('', Validators.required),
    VillageName :new FormControl('', Validators.required), 
    Contract :new FormControl(0,[Validators.required]),
    ContractPath :new FormControl(''),
    CreationDate :new FormControl(new Date().toISOString() ,[Validators.required]),
    AverageUnitRating :new FormControl(0,[Validators.required]),
    UnitAmenities:new FormControl([]),
    VerificationStatus  :new FormControl(0,[Validators.required]),
    ContractFile: new FormControl(null),
  // id :new FormControl('',[Validators.required]),
  // ownerId :new FormControl('',[Validators.required]),
  // ownerName :new FormControl('',[Validators.required]),
  // UploadDate :new FormControl(Date.now,[Validators.required]),
  // VerificationNotes :new FormControl('',[Validators.required]),
  // UnitId :new FormControl('',[Validators.required]),
 
  })

  onFileChange(event:any){
    let file = event.target.files[0];
    this.ownerForm.patchValue({ContractFile:file})
  }
  
  getFormValidationErrors() {
    const errors: any[] = [];
    Object.keys(this.ownerForm.controls).forEach(key => {
      const controlErrors = this.ownerForm.get(key)?.errors;
      if (controlErrors) {
        Object.keys(controlErrors).forEach(errorKey => {
          errors.push({
            control: key,
            error: errorKey,
            value: controlErrors[errorKey]
          });
        });
      }
    });
    return errors;
  }
  Verify(){ 
    if(this.ownerForm.invalid){
      this.ownerForm.markAllAsTouched();
      return;
    }
    
    this.ownerDTO = { ... this.ownerForm.value } as OwnerVerificationDTO;
    console.log('this.ownerDTO')
    console.log(this.ownerDTO);
    
    // this.OwnerVerificationService.VerifiyOwner(payload ).subscribe({
    const formData = new FormData();
    Object.entries(this.ownerForm.value).forEach(([key,value])=>{
      formData.append(key, value as any);
    })
    this.OwnerVerificationService.VerifiyOwner(formData).subscribe({
      next:res=>{},
      error:err=>console.log(err)
    });
  }

  get DocumentType(){
    return this.ownerForm.controls["DocumentType"];
  }
  get DocumentPath(){
    return this.ownerForm.controls["DocumentPath"];
  }
  get NationalId(){
    return this.ownerForm.controls["NationalId"];
  }
  get BankAccountDetails(){
    return this.ownerForm.controls["BankAccountDetails"];
  }
  get Title(){
    return this.ownerForm.controls["Title"];
  }
  get Description(){
    return this.ownerForm.controls["Description"];
  }
  get UnitType(){
    return this.ownerForm.controls["UnitType"];
  }
  get Bedrooms(){
    return this.ownerForm.controls["Bedrooms"];
  }
  get Bathrooms(){
    return this.ownerForm.controls["Bathrooms"];
  }
  get Sleeps(){
    return this.ownerForm.controls["Sleeps"];
  }
  get DistanceToSea(){
    return this.ownerForm.controls["DistanceToSea"];
  }
  get BasePricePerNight(){
    return this.ownerForm.controls["BasePricePerNight"];
  }
  get Address(){
    return this.ownerForm.controls["Address"];
  }
  get VillageName(){
    return this.ownerForm.controls["VillageName"];
  }
  get Contract(){
    return this.ownerForm.controls["Contract"];
  }
  get ContractPath(){
    return this.ownerForm.controls["ContractPath"];
  }
  get ContractFile(){
    return this.ownerForm.controls["ContractFile"];
  }
  // get UploadDate(){
  //   return this.ownerForm.controls["UploadDate"];
  // }
}
