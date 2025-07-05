import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:5001/api/';
  currentUser = signal<User | null>(null);

  login(model: any) {
    //return this.http.post(this.baseUrl + 'account/login', model);

    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if (user) {
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUser.set(user as User);
        }
        return user;
      })
    );
  }

  register(model: any) {
    
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUser.set(user as User);
        }
        return user;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

}
