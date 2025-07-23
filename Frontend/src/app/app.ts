import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AddUnit } from "./components/add-unit/add-unit";

import { ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chat } from './components/chat/chat';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AddUnit, Chat,ReactiveFormsModule,FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected title = 'Frontend';
}
