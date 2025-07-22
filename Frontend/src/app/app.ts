import { ReactiveFormsModule } from '@angular/forms';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Register } from './Pages/register/register';
import { Login } from './Pages/login/login';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, Login, Register,ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'VillageFront';
}
