import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
})
export class Layout implements OnInit {
  private router = inject(Router);

  userName: string = '';
  isAdmin: boolean = false;

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const tokenClaimPart = token.split('.')[1];
        const decoded = JSON.parse(atob(tokenClaimPart));

        this.userName = decoded.email;

        const role = decoded.role;

        this.isAdmin = role === 'Admin';
      } catch (error) {
        console.error('JWT decode error', error);
      }
    }
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
