import { Component } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { GameService } from './api/services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private readonly apiService: GameService) {}
  title = 'fool-game';

  async createGame() {
    alert(await firstValueFrom(this.apiService.newGame()));
  }
}
