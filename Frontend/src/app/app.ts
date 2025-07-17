import { ReactiveFormsModule } from '@angular/forms';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Login } from "./Pages/login/login";
import { HttpClientModule } from '@angular/common/http';
import { Register } from "./Pages/register/register";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Login, Register,ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'VillageFront';
}
