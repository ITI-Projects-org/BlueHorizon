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
    OwnerName:  new FormControl(''),
    FrontNationalIdDocumentPath :new FormControl(''),
    BackNationalIdDocumentPath :new FormControl(''),
    FrontNationalIdDocument :new FormControl(null),
    BackNationalIdDocument :new FormControl(null),
    
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
    UploadDate :new FormControl(new Date().toISOString()),
  // id :new FormControl('',[Validators.required]),
  // ownerId :new FormControl('',[Validators.required]),
  // ownerName :new FormControl('',[Validators.required]),
  // VerificationNotes :new FormControl('',[Validators.required]),
  // UnitId :new FormControl('',[Validators.required]),
 
  })

  onContractFileChange(event:any){
    let file = event.target.files[0];
    this.ownerForm.patchValue({ContractFile:file},
    )}
    onFrontDocumentFileChange(event:any){
    let file = event.target.files[0];
    this.ownerForm.patchValue({FrontNationalIdDocument:file},
    )}
    onBackDocumentFileChange(event:any){
    let file = event.target.files[0];
    this.ownerForm.patchValue({BackNationalIdDocument:file},
    )}
  
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
  Verify() {
    if (this.ownerForm.invalid) {
      this.ownerForm.markAllAsTouched();
      return;
    }

    const formData = new FormData();
    const formValue = this.ownerForm.value;

    // Manually append all form values EXCEPT the empty path fields
    // to work around the backend validation issue.
    for (const key in formValue) {
      if (key !== 'DocumentPath' && key !== 'ContractPath') {
        const value = formValue[key as keyof typeof formValue];
        if (value !== null && value !== undefined) {
          formData.append(key, value as any);
        }
      }
    }

    this.OwnerVerificationService.VerifiyOwner(formData).subscribe({
      next: res => {
        console.log('Verification successful!', res);
        // You can add a success message or navigate to another page here
      },
      error: err => {
        console.error('Verification failed!', err);
        // You can add logic here to display the backend error to the user
      }
    });
  }

  get DocumentType(){
    return this.ownerForm.controls["DocumentType"];
  }
  get FrontDocumentPath(){
    return this.ownerForm.controls["FrontNationalIdDocumentPath"];
  }
  get BackDocumentPath(){
    return this.ownerForm.controls["BackNationalIdDocumentPath"];
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
   get FrontDocumentFile(){
    return this.ownerForm.controls["FrontNationalIdDocument"];
  }
   get BackDocumentFile(){
    return this.ownerForm.controls["BackNationalIdDocument"];
  }
  
}
