/* tslint:disable */
/* eslint-disable */
import { Card } from './card';
import { GameStatus } from './game-status';
import { PlayerStatus } from './player-status';
import { TableCard } from './table-card';
export interface GameStateResult {
  deckCount?: number;
  hand?: null | Array<Card>;
  players?: null | Array<PlayerStatus>;
  status?: GameStatus;
  table?: null | Array<TableCard>;
}
