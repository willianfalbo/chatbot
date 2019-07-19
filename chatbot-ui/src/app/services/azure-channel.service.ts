import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { DirectLineToken } from '../models/direct-line-token.model';
import { CHATBOT_API } from '../app.setting';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AzureChannelService {

  directLineToken: DirectLineToken;

  constructor(private http: HttpClient) { }

  getDirectLineToken(): Observable<DirectLineToken> {
    return this.http.post<DirectLineToken>(`${CHATBOT_API}/azurechannel/directline/token`, {})
      .pipe(tap(result => {
        this.directLineToken = result;
      }));
  }

}
