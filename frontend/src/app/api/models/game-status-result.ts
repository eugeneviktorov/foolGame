/* tslint:disable */
/* eslint-disable */
import { GameStatus } from './game-status';
export interface GameStatusResult {
  gameId?: string;
  playersCount?: number;
  seats?: number;
  status?: GameStatus;
}
