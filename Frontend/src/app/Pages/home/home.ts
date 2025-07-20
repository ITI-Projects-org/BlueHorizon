import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-home',
  imports:[RouterLink,RouterLinkActive],
  standalone: true,
  templateUrl: './home.html',
})
export class Home {} 