import { Component, inject, OnInit, ChangeDetectorRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from './users.service';
import { User } from '../../models/user.model';
import { UserFilter } from '../../models/user-filter.model';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './users.html',
  styleUrl: './users.scss',
})
export class Users implements OnInit {
  private usersService = inject(UsersService);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  users: User[] = [];
  totalCount = 0;
  totalPages = 0;
  currentPage = 1;
  pageSize = 10;

  isFilterOpen = true;

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  closeAlert(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  filterForm: FormGroup = this.fb.group({
    name: [''],
    email: [''],
    role: ['']
  });

  isUserModalOpen = false;
  isEditMode = false;
  editingUserId: string | null = null;

  userForm: FormGroup = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.minLength(8),
      Validators.maxLength(12),
      Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&.#',])[A-Za-z\d@$!%*?&.#',]{8,}$/)
    ]],
    role: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  toggleFilter(): void {
    this.isFilterOpen = !this.isFilterOpen;
  }

  loadUsers(): void {
    const filter: UserFilter = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      name: this.filterForm.value.name || undefined,
      email: this.filterForm.value.email || undefined,
      role: this.filterForm.value.role || undefined
    };

    this.usersService.getUsers(filter).subscribe({
      next: (response: any) => {
        this.users = response.data.data;
        this.totalCount = response.data.totalCount;
        this.totalPages = response.data.totalPages;
        this.currentPage = response.data.pageNumber;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load users.', err);
      },
    });
  }

  applyFilter(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.currentPage = 1;
    this.loadUsers();
  }

  clearFilter(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.filterForm.reset({
      name: '',
      email: '',
      role: ''
    });
    this.currentPage = 1;
    this.loadUsers();
  }

  changePage(page: number | string): void {
    if (typeof page === 'number' && page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  getPageNumbers(): (number | string)[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const delta = 1;
    const range: number[] = [];
    const rangeWithDots: (number | string)[] = [];
    let l: number | undefined;

    for (let i = 1; i <= total; i++) {
      if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
        range.push(i);
      }
    }

    for (const i of range) {
      if (l) {
        if (i - l === 2) {
          rangeWithDots.push(l + 1);
        } else if (i - l !== 1) {
          rangeWithDots.push('...');
        }
      }
      rangeWithDots.push(i);
      l = i;
    }

    return rangeWithDots;
  }

  get currentStartIndex(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get currentEndIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  openAddModal(): void {
    this.isEditMode = false;
    this.editingUserId = null;
    this.userForm.reset();
    this.userForm.get('password')?.setValidators([
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(12),
      Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&.#',])[A-Za-z\d@$!%*?&.#',]{8,}$/)
    ]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.isUserModalOpen = true;
  }

  openEditModal(user: User): void {
    this.isEditMode = true;
    this.editingUserId = user.id;
    this.userForm.patchValue({
      name: user.name,
      email: user.email,
      role: user.role,
      password: ''
    });
    this.userForm.get('password')?.setValidators([
      Validators.minLength(8),
      Validators.maxLength(12),
      Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&.#',])[A-Za-z\d@$!%*?&.#',]{8,}$/)
    ]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.isUserModalOpen = true;
  }

  closeUserModal(): void {
    this.isUserModalOpen = false;
  }

  submitUserForm(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const payload = { ...this.userForm.value };
    if (this.isEditMode && !payload.password) {
      delete payload.password;
    }

    if (this.isEditMode && this.editingUserId) {
      this.usersService.updateUser(this.editingUserId, payload).subscribe({
        next: () => {
          this.successMessage.set('User updated successfully.');
          this.closeUserModal();
          this.loadUsers();
        },
        error: (err) => {
          console.error('Failed to update user', err);
          const msg = err.error?.detail || err.error?.title || err.error?.message || 'Failed to update user. Please check all required fields.';
          this.errorMessage.set(msg);
        }
      });
    } else {
      this.usersService.createUser(payload).subscribe({
        next: () => {
          this.successMessage.set('User added successfully.');
          this.closeUserModal();
          this.loadUsers();
        },
        error: (err) => {
          console.error('Failed to create user', err);
          const msg = err.error?.detail || err.error?.title || err.error?.message || 'Failed to add user. Please check all required fields.';
          this.errorMessage.set(msg);
        }
      });
    }
  }

  isDeleteModalOpen = false;
  deletingUser: User | null = null;

  openDeleteModal(user: User): void {
    this.deletingUser = user;
    this.isDeleteModalOpen = true;
  }

  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.deletingUser = null;
  }

  confirmDelete(): void {
    if (this.deletingUser) {
      this.usersService.deleteUser(this.deletingUser.id).subscribe({
        next: () => {
          this.successMessage.set('User and associated properties deleted successfully.');
          this.loadUsers();
          this.closeDeleteModal();
        },
        error: (err) => {
          console.error('Failed to delete user', err);
          this.errorMessage.set('Failed to delete user.');
          this.closeDeleteModal();
        }
      });
    }
  }
}
