import {
  ChangeDetectorRef,
  Component,
  OnInit,
  HostListener,
} from '@angular/core';
import { QrServise } from '../../Services/qr-service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { QrCodeDto } from '../../Models/qr-code-dto';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-qr',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-qr.html',
  styleUrl: './create-qr.css',
})
export class CreateQr implements OnInit {
  qrForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;
  imgPath: string | null = null;
  isFormVisible = false;

  constructor(
    private qrService: QrServise,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.qrForm = this.fb.group({
      BookingId: ['', [Validators.required, Validators.min(1)]],
      TenantNationalId: ['', [Validators.required, Validators.minLength(10)]],
      VillageName: ['', Validators.required],
      UnitAddress: ['', Validators.required],
      OwnerName: ['', Validators.required],
      TenantName: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    // Any initialization logic
  }

  // Form control getters for easier access in template
  get BookingId() {
    return this.qrForm.get('BookingId');
  }
  get TenantNationalId() {
    return this.qrForm.get('TenantNationalId');
  }
  get VillageName() {
    return this.qrForm.get('VillageName');
  }
  get UnitAddress() {
    return this.qrForm.get('UnitAddress');
  }
  get OwnerName() {
    return this.qrForm.get('OwnerName');
  }
  get TenantName() {
    return this.qrForm.get('TenantName');
  }

  onSubmit() {
    if (this.qrForm.valid) {
      this.isLoading = true;
      this.errorMessage = null;
      const qrData: QrCodeDto = this.qrForm.value;

      this.qrService.createQrCloud(qrData).subscribe({
        next: (response) => {
          this.imgPath = response.imgPath;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to create QR code';
          this.isLoading = false;
          this.cdr.detectChanges();
        },
      });
    } else {
      this.errorMessage = 'Please fill in all required fields correctly';
      this.qrForm.markAllAsTouched();
    }
  }

  reset() {
    this.qrForm.reset();
    this.errorMessage = null;
    this.imgPath = null;
  }

  showForm() {
    this.isFormVisible = true;
  }

  hideForm() {
    this.isFormVisible = false;
    this.reset();
  }

  // Close overlay when clicking outside
  @HostListener('document:keydown.escape')
  onEscapePress() {
    this.hideForm();
  }
}
