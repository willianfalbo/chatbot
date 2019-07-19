import { Component, ElementRef, ViewChild, AfterViewInit, OnInit } from '@angular/core';

import { UserLocation } from './models/user-location.model';
import { AzureChannelService } from './services/azure-channel.service';

import { DirectLine, ConnectionStatus } from 'botframework-directlinejs';
import { Guid } from 'guid-typescript';
import * as ms from 'milliseconds';

/**
 * Declares the WebChat property on the window object.
 */
declare global {
  interface Window {
    WebChat: any;
  }
}

window.WebChat = window.WebChat || {};

// constants
const LOCALE_LANG = 'pt-BR';
const USER_ID_NAME = 'userID';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewInit {

  @ViewChild('botWindow', { static: false }) botWindowElement: ElementRef;
  userId: string;
  userLocation: UserLocation = new UserLocation(null, null, false);

  constructor(private dlService: AzureChannelService) {
    this.userId = this.getUserGuid();
  }

  ngOnInit(): void {
    this.handleUserGeolocation();
  }

  ngAfterViewInit(): void {

    this.dlService.getDirectLineToken()
      .subscribe(result => {
        console.log('DIRECT LINE SERVICE RESULT: ', result);
        this.renderWebChat();
      }, error => {
        console.error('DIRECT LINE SERVICE ERROR: ', error);
      }, () => {
        console.log('DIRECT LINE SERVICE COMPLETED');
      });

  }

  private renderWebChat() {
    const dl = this.setupDirectLine();
    this.postJoinActivity(dl);
    this.listenToActivities(dl);
    this.monitorConnection(dl);
  }

  // post activities to the bot
  // https://github.com/microsoft/BotFramework-DirectLineJS
  private postJoinActivity(dl: DirectLine) {
    dl.postActivity({
      from: { id: this.userId },
      type: 'event',
      name: 'chatbot/join',
      value: { language: LOCALE_LANG }
    }).subscribe(id =>
      console.log('Posted activity, assigned ID', id),
      error => console.log('Error posting activity', error)
    );
  }

  // listen to activities sent from the bot
  // https://github.com/microsoft/BotFramework-DirectLineJS
  private listenToActivities(dl: DirectLine) {
    dl.activity$
      .subscribe(activity => {
        console.log('received activity ', activity);
      });
  }

  // monitor connection status
  // https://github.com/microsoft/BotFramework-DirectLineJS
  private monitorConnection(dl: DirectLine) {
    dl.connectionStatus$
      .subscribe(connectionStatus => {
        switch (connectionStatus) {
          // the status when the DirectLine object is first created/constructed
          case ConnectionStatus.Uninitialized: {
            console.log('The app connection is UNINITIALIZED');
            break;
          }
          // currently trying to connect to the conversation
          case ConnectionStatus.Connecting: {
            console.log('The app is CONNECTING');
            break;
          }
          // successfully connected to the converstaion. Connection is healthy so far as we know.
          case ConnectionStatus.Online: {
            console.log('The app is ONLINE');
            break;
          }
          // last operation errored out with an expired token. Your app should supply a new one.
          case ConnectionStatus.ExpiredToken: {
            console.error('The app connection TOKEN IS EXPIRED');
            break;
          }
          // the initial attempt to connect to the conversation failed. No recovery possible.
          case ConnectionStatus.FailedToConnect: {
            console.error('The app FAILED TO CONNECT');
            break;
          }
          // the bot ended the conversation
          case ConnectionStatus.Ended: {
            console.log('The app connection is Ended');
            break;
          }
        }
      });
  }

  // https://github.com/microsoft/BotFramework-DirectLineJS
  private setupDirectLine() {

    const styleOptions = this.styleOptionsSetup();

    const dl = new DirectLine({
      token: this.dlService.directLineToken.value.token,
      webSocket: true
    });

    // middleware
    const store = this.webChatMiddleware();

    // https://github.com/microsoft/BotFramework-WebChat
    window.WebChat.renderWebChat({
      directLine: dl,
      store,
      userID: this.userId,
      // to enable speech button
      webSpeechPonyfillFactory: window.WebChat.createBrowserWebSpeechPonyfillFactory({
        region: LOCALE_LANG
      }),
      styleOptions,
      locale: LOCALE_LANG,
      sendTypingIndicator: true
    }, this.botWindowElement.nativeElement);

    return dl;
  }

  private webChatMiddleware() {
    return window.WebChat.createStore({}, ({ dispatch }) => next => action => {
      // console.log('ACTION RECEIVED: ', action);
      if (action.type === 'DIRECT_LINE/POST_ACTIVITY_PENDING') {
        if (action.payload.activity.type === 'message') {
          // append "userLocation" object to "channelData"
          Object.assign(action.payload.activity.channelData, { userLocation: this.userLocation });
        }
      }
      return next(action);
    });
  }

  private getUserGuid(): string {
    const userGuidId = localStorage.getItem(USER_ID_NAME);
    if (userGuidId && Guid.isGuid(userGuidId)) {
      return userGuidId;
    } else {
      const newGuid = Guid.create().toString();
      localStorage.setItem(USER_ID_NAME, newGuid);
    }
  }

  // customize webchat
  // https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-webchat-customization?view=azure-bot-service-4.0
  // https://github.com/Microsoft/BotFramework-WebChat/blob/master/packages/component/src/Styles/defaultStyleOptions.js
  styleOptionsSetup() {
    const DEFAULT_FONT = 'Calibri';
    const DEFAULT_BORDER_RADIUS = 15;
    return {
      // fonts
      monospaceFont: DEFAULT_FONT,
      primaryFont: DEFAULT_FONT,
      // avatar
      botAvatarImage: '/assets/img/avatar-face2.png',
      // userAvatarImage: '',
      bubbleBackground: 'rgba(88, 88, 88, 0.06)',
      bubbleFromUserBackground: 'rgba(56, 88, 205, 1)',
      // bubble
      bubbleBorderRadius: DEFAULT_BORDER_RADIUS,
      bubbleFromUserBorderRadius: DEFAULT_BORDER_RADIUS,
      bubbleBorder: '',
      bubbleFromUserBorder: '',
      // bubbleTextColor: 'Black',
      bubbleFromUserTextColor: 'White',
      // Suggested actions
      suggestedActionBorderRadius: DEFAULT_BORDER_RADIUS,
    };
  }

  // this method was created based on
  // https://www.w3schools.com/html/html5_geolocation.asp
  // https://developers.google.com/web/fundamentals/native-hardware/user-location/
  private handleUserGeolocation() {

    const dataCachedTime = ms.minutes(1);
    const waitingTime = ms.seconds(15);

    if (navigator.geolocation) {
      // navigator.geolocation.getCurrentPosition(
      navigator.geolocation.watchPosition(
        this.geoSuccess.bind(this),
        this.geoError.bind(this),
        { enableHighAccuracy: true, maximumAge: dataCachedTime, timeout: waitingTime }
      );
    } else {
      // Geolocation is not supported by this browser.
      alert('Geolocalização não oferece suporte para este navegador.');
    }

  }

  private geoSuccess(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;

    this.userLocation = new UserLocation(latitude, longitude, true);
    console.log('USER LOCATION', this.userLocation);
  }

  private geoError(error) {
    switch (error.code) {
      // User denied the request for Geolocation
      case error.PERMISSION_DENIED:
        console.error('User denied the request for Geolocation');
        this.userLocation = new UserLocation(null, null, false);
        break;
      // Information is unavailable
      case error.POSITION_UNAVAILABLE:
        console.error('Location information is unavailable');
        break;
      // The request to get user location timed out
      case error.TIMEOUT:
        console.error('The request to get user location timed out');
        this.handleUserGeolocation();
        break;
      // An unknown error occurred
      case error.UNKNOWN_ERROR:
        console.error('An unknown error occurred');
        break;
    }
  }

}
