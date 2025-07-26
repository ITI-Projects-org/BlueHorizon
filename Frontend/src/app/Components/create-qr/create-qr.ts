import { ChangeDetectorRef, Component } from '@angular/core';
import { QrServise } from '../../Services/qr-service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-create-qr',
  imports: [],
  templateUrl: './create-qr.html',
  styleUrl: './create-qr.css',
})
export class CreateQr {
  constructor(
    private qrService: QrServise,
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

  // ------------------------
  // ------------------------
  // ------------------------
  // ---------CLOUD----------
  // ------------------------
  // ------------------------
  imgPath!: string;
  CreateQrCloud() {
    console.log('from post cloud .ts');
    this.qrService.createQrCloud().subscribe({
      next: (response) => {
        console.log(response);
        this.imgPath = response.imgPath;
        this.cdr.detectChanges();
      },
    });
  }
  getQrCloud(qrId: number) {
    console.log('from get cloud .ts');

    const getSub = this.qrService.getQrCodeCloud(qrId).subscribe({
      next: (res) => {
        this.imgPath = res.imgPath;
        this.cdr.detectChanges();
        this.cdr.detectChanges();
      },
    });
  }
}
