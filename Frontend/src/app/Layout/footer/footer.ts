import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-footer',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
})
export class Footer {
  subscribeNewsletter(event: Event): void {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const emailInput = form.querySelector(
      'input[type="email"]'
    ) as HTMLInputElement;

    if (emailInput && emailInput.value) {
      // Here you would typically send the email to your backend
      console.log('Newsletter subscription for:', emailInput.value);
      alert('Thank you for subscribing to our newsletter!');
      emailInput.value = '';
    }
  }
}
