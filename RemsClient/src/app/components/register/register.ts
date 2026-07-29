import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  registerForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.minLength(8),
      Validators.maxLength(12),
      Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&.#',])[A-Za-z\d@$!%*?&.#',]{8,}$/),
    ]),
  });

  private authService = inject(AuthService);
  private router = inject(Router);
  errorMessage = signal<string | null>(null);
  isRegistered = signal(false);

  onSubmit() {
    this.authService
      .register({
        name: this.registerForm.value.name!,
        email: this.registerForm.value.email!,
        password: this.registerForm.value.password!,
      })
      .subscribe({
        next: (response: any) => {
          const token = response.token;
          localStorage.setItem('token', token);
          this.isRegistered.set(true);
          this.errorMessage.set(null);
          this.registerForm.reset();
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1000);
        },
        error: (err) => {
          console.log(err);
          this.errorMessage.set(err.error);
        },
      });
  }

  closeAlert() {
    this.errorMessage.set(null);
    this.isRegistered.set(false);
  }
}
