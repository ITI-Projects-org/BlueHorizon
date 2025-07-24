import { ChangeDetectorRef, Component } from '@angular/core';
import { QrService } from '../../Services/qr-service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-create-qr',
  imports: [],
  templateUrl: './create-qr.html',
  styleUrl: './create-qr.css',
})
export class CreateQr {
  constructor(
    private qrService: QrService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) {}
  qrCodeUrl: SafeUrl | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  getQr(qrId: number) {
    const getSub = this.qrService.getQrCode(qrId).subscribe({
      next: (imageBlob) => {
        const objectUrl = URL.createObjectURL(imageBlob);
        this.qrCodeUrl = this.sanitizer.bypassSecurityTrustUrl(objectUrl);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  createQR() {
    console.log('btn clicked');
    this.qrService.createQr().subscribe({
      next: (response) => {
        const getSub = this.qrService.getQrCode(response.qrId).subscribe({
          next: (imageBlob) => {
            const objectUrl = URL.createObjectURL(imageBlob);
            this.qrCodeUrl = this.sanitizer.bypassSecurityTrustUrl(objectUrl);
            this.isLoading = false;
            this.cdr.detectChanges();
          },
        });
      },
    });
  }
}
