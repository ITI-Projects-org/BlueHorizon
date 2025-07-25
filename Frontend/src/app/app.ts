import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet ,ReactiveFormsModule,FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected title = 'Frontend';
}
