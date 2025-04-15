import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from "rxjs";

interface Forecast {
    date: string,
    temperatureC: number,
    temperatureF: number,
    summary: string,
}

interface UserInfo {
    name: string;
    email: string;
    picture: string;
}

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css'
})
export class AppComponent {
    title = 'ClientApp';

    httpClient = inject(HttpClient);
    weather = signal<Forecast[]>([]);
    userInfo = signal<UserInfo | null>(null);

    async ngOnInit() {
        const response = await lastValueFrom(this.httpClient.get<Forecast[]>('api/weatherforecast'));
        console.log(response);
        this.weather.set(response);
    }

    http = inject(HttpClient);

    onLogin() {
          window.location.href = `https://localhost:7169/api/login`;
    }

    onLogout() {
        window.location.href = `https://localhost:7169/api/logout`;
    }

    onLogoutGoogle() {
        window.location.href = 'https://accounts.google.com/Logout';
    }
    async getUser() {
        const user = await lastValueFrom(this.http.get<UserInfo>('/api/user'));
        console.log(user);
        this.userInfo.set(user);


    }

}
