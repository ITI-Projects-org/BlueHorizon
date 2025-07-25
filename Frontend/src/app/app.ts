import { Component, signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { Footer } from "./Layout/footer/footer";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [RouterOutlet, Footer],
  templateUrl: "./app.html",
  styleUrls: ["./app.css"],
})
export class App {
  protected readonly title = signal("Frontend");
}
