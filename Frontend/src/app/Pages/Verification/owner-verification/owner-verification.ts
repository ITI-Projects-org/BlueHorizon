import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Verification } from '../../../Services/verification-service';
import { OwnerVerificationDTO } from '../../../Models/owner-verification';
import { Amenity } from '../../../Models/Amenity';

@Component({
  selector: 'app-owner-verification',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './owner-verification.html',
  styleUrl: './owner-verification.css',
})
export class OwnerVerification implements OnInit {
  constructor(private OwnerVerificationService: Verification) {}
  selectedAmenityIds: number[] = [];
  ownerDTO!: OwnerVerificationDTO;
  amenities: Amenity[] = [];
  unitImages: File[] = []; // إضافة متغير جديد لتخزين ملفات الصور المتعددة

  ownerForm = new FormGroup({
    DocumentType: new FormControl(0, [Validators.required]),
    OwnerName: new FormControl(''),
    FrontNationalIdDocumentPath: new FormControl(''),
    BackNationalIdDocumentPath: new FormControl(''),
    FrontNationalIdDocument: new FormControl(null, [Validators.required]), // أضف Validators.required
    BackNationalIdDocument: new FormControl(null, [Validators.required]),   // أضف Validators.required

    NationalId: new FormControl('', [Validators.required]),
    BankAccountDetails: new FormControl('', [Validators.required]),
    Title: new FormControl('', [Validators.required]),
    Description: new FormControl('', [Validators.required]), // أضف Validators.required
    UnitType: new FormControl(0, [Validators.required]), // أضف Validators.required
    Bedrooms: new FormControl(0, [Validators.required]),   // أضف Validators.required
    Bathrooms: new FormControl(0, [Validators.required]),  // أضف Validators.required
    Sleeps: new FormControl(0, [Validators.required]),     // أضف Validators.required
    DistanceToSea: new FormControl(0, [Validators.required]), // أضف Validators.required
    BasePricePerNight: new FormControl(0, [Validators.required]), // أضف Validators.required
    Address: new FormControl('', Validators.required),
    VillageName: new FormControl('', Validators.required),
    Contract: new FormControl(0), // غالبًا ده مش هيكون مطلوب في الفورم نفسها لكن كـ ID لو موجود في الـ DTO
    ContractPath: new FormControl(''),
    CreationDate: new FormControl(new Date().toISOString(), [
      Validators.required,
    ]),
    AverageUnitRating: new FormControl(0, [Validators.required]),
    UnitAmenities: new FormControl([]),
    VerificationStatus: new FormControl(0, [Validators.required]),
    ContractFile: new FormControl(null, [Validators.required]), // أضف Validators.required
    UploadDate: new FormControl(new Date().toISOString()),
    UnitImages: new FormControl<File[] | null>(null, [Validators.required]), // إضافة FormControl لصور الوحدة
  });

  ngOnInit(): void {
    this.OwnerVerificationService.GetAllAmenities().subscribe({
      next: (res) => {
        console.log('Amenities loaded =');
        console.log(res);
        this.amenities = res as unknown as Amenity[];
      },
      error: (e) => console.log(e),
    });
  }

  onAmenityChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    const id = parseInt(checkbox.value, 10);
    if (checkbox.checked) {
      this.selectedAmenityIds.push(id);
    } else {
      this.selectedAmenityIds = this.selectedAmenityIds.filter((a) => a !== id);
    }
  }

  onContractFileChange(event: any) {
    let file = event.target.files[0];
    this.ownerForm.patchValue({ ContractFile: file });
  }

  onFrontDocumentFileChange(event: any) {
    let file = event.target.files[0];
    this.ownerForm.patchValue({ FrontNationalIdDocument: file });
  }

  onBackDocumentFileChange(event: any) {
    let file = event.target.files[0];
    this.ownerForm.patchValue({ BackNationalIdDocument: file });
  }

  // دالة جديدة للتعامل مع اختيار صور الوحدة
  onUnitFileSelected(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      this.unitImages = Array.from(event.target.files);
      this.ownerForm.patchValue({ UnitImages: this.unitImages }); // تحديث FormControl بقائمة الملفات
    } else {
      this.unitImages = [];
      this.ownerForm.patchValue({ UnitImages: null }); // إفراغ FormControl إذا لم يتم اختيار ملفات
    }
  }

  getFormValidationErrors() {
    const errors: any[] = [];
    Object.keys(this.ownerForm.controls).forEach((key) => {
      const controlErrors = this.ownerForm.get(key)?.errors;
      if (controlErrors) {
        Object.keys(controlErrors).forEach((errorKey) => {
          errors.push({
            control: key,
            error: errorKey,
            value: controlErrors[errorKey],
          });
        });
      }
    });
    return errors;
  }

  Verify() {
    if (this.ownerForm.invalid) {
      this.ownerForm.markAllAsTouched();
      console.log('Form is invalid. Errors:', this.getFormValidationErrors()); // log errors
      return;
    }

    const formData = new FormData();
    const formValue = this.ownerForm.value;

    // Manually append all form values EXCEPT the empty path fields
    for (const key in formValue) {
      if (key !== 'ContractPath' && key !== 'FrontNationalIdDocumentPath' && key !== 'BackNationalIdDocumentPath' && key !== 'UnitImages') {
        const value = formValue[key as keyof typeof formValue];
        if (value !== null && value !== undefined) {
          formData.append(key, value as any);
        }
      }
    }

    // Append file objects separately
    if (formValue.FrontNationalIdDocument) {
      formData.append('FrontNationalIdDocument', formValue.FrontNationalIdDocument);
    }
    if (formValue.BackNationalIdDocument) {
      formData.append('BackNationalIdDocument', formValue.BackNationalIdDocument);
    }
    if (formValue.ContractFile) {
      formData.append('ContractFile', formValue.ContractFile);
    }

    // Append multiple unit images
    this.unitImages.forEach((file) => {
      formData.append('UnitImages', file, file.name); // 'UnitImages' هو اسم الـ field اللي الـ API هيستقبله لملفات الصور
    });


    this.selectedAmenityIds.forEach((id) => {
      formData.append('UnitAmenities', id.toString()); // هنا استخدمت 'UnitAmenities' بناءً على اسم الـ FormControl
      console.log('Selected Amenity ID:', id);
    });

    this.selectedAmenityIds = [];

    this.OwnerVerificationService.VerifiyOwner(formData).subscribe({
      next: (res) => {
        console.log('Verification successful!', res);
        // You can add a success message or navigate to another page here
        this.ownerForm.reset(); // ممكن تعمل reset للفورم بعد النجاح
        this.unitImages = []; // إفراغ قائمة الصور بعد الإرسال
      },
      error: (err) => {
        console.error('Verification failed!', err);
        // You can add logic here to display the backend error to the user
      },
    });
  }

  get DocumentType() {
    return this.ownerForm.controls['DocumentType'];
  }
  get FrontDocumentPath() {
    return this.ownerForm.controls['FrontNationalIdDocumentPath'];
  }
  get BackDocumentPath() {
    return this.ownerForm.controls['BackNationalIdDocumentPath'];
  }
  get NationalId() {
    return this.ownerForm.controls['NationalId'];
  }
  get BankAccountDetails() {
    return this.ownerForm.controls['BankAccountDetails'];
  }
  get Title() {
    return this.ownerForm.controls['Title'];
  }
  get Description() {
    return this.ownerForm.controls['Description'];
  }
  get UnitType() {
    return this.ownerForm.controls['UnitType'];
  }
  get Bedrooms() {
    return this.ownerForm.controls['Bedrooms'];
  }
  get Bathrooms() {
    return this.ownerForm.controls['Bathrooms'];
  }
  get Sleeps() {
    return this.ownerForm.controls['Sleeps'];
  }
  get DistanceToSea() {
    return this.ownerForm.controls['DistanceToSea'];
  }
  get BasePricePerNight() {
    return this.ownerForm.controls['BasePricePerNight'];
  }
  get Address() {
    return this.ownerForm.controls['Address'];
  }
  get VillageName() {
    return this.ownerForm.controls['VillageName'];
  }
  get Contract() {
    return this.ownerForm.controls['Contract'];
  }
  get ContractPath() {
    return this.ownerForm.controls['ContractPath'];
  }
  get ContractFile() {
    return this.ownerForm.controls['ContractFile'];
  }
  get FrontDocumentFile() {
    return this.ownerForm.controls['FrontNationalIdDocument'];
  }
  get BackDocumentFile() {
    return this.ownerForm.controls['BackNationalIdDocument'];
  }
  get UnitImages() {
    return this.ownerForm.controls['UnitImages'];
  }
}
