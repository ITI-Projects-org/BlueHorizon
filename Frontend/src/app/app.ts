import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AddUnit } from "./components/add-unit/add-unit";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AddUnit],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'Frontend';
}
