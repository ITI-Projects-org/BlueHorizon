import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from './Layout/footer/footer';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Navbar } from './Layout/navbar/navbar';
import { AIChatComponent } from './Components/ai-chat/ai-chat';
import { ChatComponent } from "./Components/chat/chat";

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    Footer,
    Navbar,
    FormsModule,
    ReactiveFormsModule,
    AIChatComponent,
    ChatComponent
],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Frontend');
}
