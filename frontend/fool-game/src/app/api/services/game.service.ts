/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpContext } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { GameStateResult } from '../models/game-state-result';
import { GameStatusResult } from '../models/game-status-result';

@Injectable({
  providedIn: 'root',
})
export class GameService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiGameNewPost
   */
  static readonly ApiGameNewPostPath = '/api/game/new';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `newGame$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  newGame$Plain$Response(params?: {
    playersCount?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameNewPostPath, 'post');
    if (params) {
      rb.query('playersCount', params.playersCount, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `newGame$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  newGame$Plain(params?: {
    playersCount?: number;
    context?: HttpContext
  }
): Observable<string> {

    return this.newGame$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `newGame()` instead.
   *
   * This method doesn't expect any request body.
   */
  newGame$Response(params?: {
    playersCount?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameNewPostPath, 'post');
    if (params) {
      rb.query('playersCount', params.playersCount, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `newGame$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  newGame(params?: {
    playersCount?: number;
    context?: HttpContext
  }
): Observable<string> {

    return this.newGame$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * Path part for operation apiGameListGet
   */
  static readonly ApiGameListGetPath = '/api/game/list';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `listGames$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  listGames$Plain$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<GameStatusResult>>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameListGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<GameStatusResult>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `listGames$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  listGames$Plain(params?: {
    context?: HttpContext
  }
): Observable<Array<GameStatusResult>> {

    return this.listGames$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Array<GameStatusResult>>) => r.body as Array<GameStatusResult>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `listGames()` instead.
   *
   * This method doesn't expect any request body.
   */
  listGames$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<GameStatusResult>>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameListGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<GameStatusResult>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `listGames$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  listGames(params?: {
    context?: HttpContext
  }
): Observable<Array<GameStatusResult>> {

    return this.listGames$Response(params).pipe(
      map((r: StrictHttpResponse<Array<GameStatusResult>>) => r.body as Array<GameStatusResult>)
    );
  }

  /**
   * Path part for operation apiGameJoinPost
   */
  static readonly ApiGameJoinPostPath = '/api/game/join';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `joinGame$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  joinGame$Plain$Response(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameJoinPostPath, 'post');
    if (params) {
      rb.query('gameId', params.gameId, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `joinGame$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  joinGame$Plain(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<string> {

    return this.joinGame$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `joinGame()` instead.
   *
   * This method doesn't expect any request body.
   */
  joinGame$Response(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameJoinPostPath, 'post');
    if (params) {
      rb.query('gameId', params.gameId, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `joinGame$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  joinGame(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<string> {

    return this.joinGame$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * Path part for operation apiGameStartPost
   */
  static readonly ApiGameStartPostPath = '/api/game/start';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `startGame()` instead.
   *
   * This method doesn't expect any request body.
   */
  startGame$Response(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameStartPostPath, 'post');
    if (params) {
      rb.query('gameId', params.gameId, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `startGame$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  startGame(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<void> {

    return this.startGame$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation apiGamePlayPost
   */
  static readonly ApiGamePlayPostPath = '/api/game/play';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `play$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  play$Plain$Response(params?: {
    token?: string;
    cardIndex?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGamePlayPostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
      rb.query('cardIndex', params.cardIndex, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `play$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  play$Plain(params?: {
    token?: string;
    cardIndex?: number;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.play$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `play()` instead.
   *
   * This method doesn't expect any request body.
   */
  play$Response(params?: {
    token?: string;
    cardIndex?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGamePlayPostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
      rb.query('cardIndex', params.cardIndex, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `play$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  play(params?: {
    token?: string;
    cardIndex?: number;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.play$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * Path part for operation apiGameStateGet
   */
  static readonly ApiGameStateGetPath = '/api/game/state';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `stateGame$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  stateGame$Plain$Response(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameStateGetPath, 'get');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `stateGame$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  stateGame$Plain(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.stateGame$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `stateGame()` instead.
   *
   * This method doesn't expect any request body.
   */
  stateGame$Response(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameStateGetPath, 'get');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `stateGame$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  stateGame(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.stateGame$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * Path part for operation apiGameBeatPost
   */
  static readonly ApiGameBeatPostPath = '/api/game/beat';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `beat$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  beat$Plain$Response(params?: {
    token?: string;
    handCardIndex?: number;
    tableCardIndex?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameBeatPostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
      rb.query('handCardIndex', params.handCardIndex, {"style":"form"});
      rb.query('tableCardIndex', params.tableCardIndex, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `beat$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  beat$Plain(params?: {
    token?: string;
    handCardIndex?: number;
    tableCardIndex?: number;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.beat$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `beat()` instead.
   *
   * This method doesn't expect any request body.
   */
  beat$Response(params?: {
    token?: string;
    handCardIndex?: number;
    tableCardIndex?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameBeatPostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
      rb.query('handCardIndex', params.handCardIndex, {"style":"form"});
      rb.query('tableCardIndex', params.tableCardIndex, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `beat$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  beat(params?: {
    token?: string;
    handCardIndex?: number;
    tableCardIndex?: number;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.beat$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * Path part for operation apiGameTakePost
   */
  static readonly ApiGameTakePostPath = '/api/game/take';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `take$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  take$Plain$Response(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameTakePostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `take$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  take$Plain(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.take$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `take()` instead.
   *
   * This method doesn't expect any request body.
   */
  take$Response(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<GameStateResult>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameTakePostPath, 'post');
    if (params) {
      rb.query('token', params.token, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<GameStateResult>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `take$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  take(params?: {
    token?: string;
    context?: HttpContext
  }
): Observable<GameStateResult> {

    return this.take$Response(params).pipe(
      map((r: StrictHttpResponse<GameStateResult>) => r.body as GameStateResult)
    );
  }

  /**
   * Path part for operation apiGameEndRoundPost
   */
  static readonly ApiGameEndRoundPostPath = '/api/game/end_round';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `endRound()` instead.
   *
   * This method doesn't expect any request body.
   */
  endRound$Response(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, GameService.ApiGameEndRoundPostPath, 'post');
    if (params) {
      rb.query('gameId', params.gameId, {"style":"form"});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `endRound$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  endRound(params?: {
    gameId?: string;
    context?: HttpContext
  }
): Observable<void> {

    return this.endRound$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
