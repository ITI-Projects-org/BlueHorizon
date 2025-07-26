import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from './Layout/footer/footer';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UnitDetailsComponent } from './Components/unit-details/unit-details';
import { NavbarComponent } from './Layout/navbar/navbar';
import { ChatComponent } from './Components/chat/chat';
@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    Footer,
    NavbarComponent,ChatComponent,
    FormsModule,
    ReactiveFormsModule,
    UnitDetailsComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Frontend');
}
