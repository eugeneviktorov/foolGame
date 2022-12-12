import { Component, OnInit } from '@angular/core';
import {
  BehaviorSubject,
  firstValueFrom,
  isObservable,
  Observable,
} from 'rxjs';
import { GameStatusResult } from './api/models';
import { GameService } from './api/services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private readonly apiService: GameService) {}

  title = 'fool-game';

  games$ = new BehaviorSubject<GameStatusResult[]>([]);

  ngOnInit(): void {}

  async createGame() {
    this.apiService.newGame({ playersCount: 4 }).subscribe((gameId) => {
      alert(gameId);

      this.apiService.listGames().subscribe((y) => this.games$.next(y));
      this.apiService.joinGame({gameId : gameId}).subscribe(() => alert('subscribed'));
      this.apiService.listGames().subscribe((y) => this.games$.next(y));
    });
  }
}
