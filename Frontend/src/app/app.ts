import { ReactiveFormsModule } from '@angular/forms';
import { Component } from '@angular/core';
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './Layout/navbar/navbar';
import { Footer } from "./Layout/footer/footer";
import { Register } from './Pages/register/register';
import { Login } from './Pages/login/login';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chat } from './components/chat/chat';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Footer, Navbar Login, Register, ReactiveFormsModule, AddUnit, Chat, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Frontend');
}
