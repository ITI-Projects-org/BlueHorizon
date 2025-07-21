import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { LoginDTO } from '../Models/loginDTO';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { RegisterDTO } from '../Models/register-dto';
import { isPlatformBrowser } from '@angular/common';


@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  loginDTO!: LoginDTO;
  registerDTO!:RegisterDTO;
  authUrl: string = 'https://localhost:7083/api/Authentication';

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) private platformId: Object) {}

  getUserName(token: string) :string | undefined {
    try {
      const decoded: any = jwtDecode(token);
      const userNameClaim = decoded['username'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/username'];
      return userNameClaim?.toString();
    } catch {
      return 'Guest';
    }
  }
  getRole(token: string) :string | undefined {
    try {
      const decoded: any = jwtDecode(token);
      const roleClaim = decoded['role'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      if (Array.isArray(roleClaim)) {
        return roleClaim[0]; 
      }
      return roleClaim?.toString();
    } catch {
      return '';
    }
  }
  login(loginDTO:LoginDTO): Observable<{ token: string }> {
    let observable;
    // delete this
    // this.loginDTO = {
    //   email: 'ElSabagh@gmail.com',
    //   role: 'Owner',
    //   password: '123',
    //   username: 'Mohamed_ElSabagh',
      
    // };

    return this.http
      .post<{ token: string }>(`${this.authUrl}/Login`, loginDTO)
      .pipe(tap((res) => {
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('token', res.token);
          localStorage.setItem('role',this.getRole(res.token)?.toString() ?? '');
          localStorage.setItem('username',this.getUserName(res.token)?.toString()??'Guest')
        }
      }));
  }
  register(registerDTO:RegisterDTO):Observable<any>{
    // this.registerDTO = {
    //   email: 'ElSabagh2@gmail.com',
    //   role: 'Admin',
    //   password: '123',
    //   username: 'Mohamed_ElSabagh',
    //   confirmPassword: '123',
    // };
    return this.http.post<any>(`${this.authUrl}/Register`,registerDTO, {headers: new HttpHeaders({"Content-Type":"application/json"})})
    .pipe(tap (res=>{console.log(res)}))
  }
}


