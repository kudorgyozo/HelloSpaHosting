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

  async ngOnInit() {
    const response = await lastValueFrom(this.httpClient.get<Forecast[]>('api/weatherforecast'));
    console.log(response);
    this.weather.set(response);
  }
}
