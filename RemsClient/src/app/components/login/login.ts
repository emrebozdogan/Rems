import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });
  errorMessage = signal<string | null>(null);
  private authService = inject(AuthService);
  private router = inject(Router);
  loggedIn = signal(false);

  onSubmit() {
    this.authService
      .login({
        email: this.loginForm.value.email!,
        password: this.loginForm.value.password!,
      })
      .subscribe({
        next: (response: any) => {
          const token = response.token;
          localStorage.setItem('token', token);
          this.errorMessage.set(null);
          this.loggedIn.set(true);
          setTimeout(() => {
            this.router.navigate(['/properties']);
          }, 1000);
        },
        error: (err) => {
          console.log(err);
          this.errorMessage.set('Wrong email or password');
        },
      });
  }

  closeAlert() {
    this.errorMessage.set(null);
    this.loggedIn.set(false);
  }
}
