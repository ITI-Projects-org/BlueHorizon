import { ReactiveFormsModule } from '@angular/forms';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Register } from './Pages/register/register';
import { Login } from './Pages/login/login';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chat } from './components/chat/chat';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Login, Register, ReactiveFormsModule, AddUnit, Chat, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected title = 'VillageFront';
}
