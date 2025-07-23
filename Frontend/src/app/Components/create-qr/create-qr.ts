import { Component } from '@angular/core';
import { QrServise } from '../../Services/qr-servise';

@Component({
  selector: 'app-create-qr',
  imports: [],
  templateUrl: './create-qr.html',
  styleUrl: './create-qr.css'
})
export class CreateQr {
  constructor(private qrService:QrServise){}
  createQR(){
    try{
    console.log("btn clicked")
    this.qrService.createQr()
      .subscribe({
        next:e=>console.log(e),
        error:e=>console.log(e)
      })
}
  catch(e){
    console.log(e) 
  }  
  }
}
