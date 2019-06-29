import { Component, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { DirectLine } from 'botframework-directlinejs';
import { Guid } from 'guid-typescript';

/**
 * Declares the WebChat property on the window object.
 */
declare global {
  interface Window {
    WebChat: any;
  }
}

window.WebChat = window.WebChat || {};

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit {

  @ViewChild('botWindow', { static: false }) botWindowElement: ElementRef;
  userId: string;

  constructor() {
    this.userId = this.GetOrSetGuid();
  }

  ngAfterViewInit(): void {

    const directLine = this.ChatbotSetup();

    // post activities to the bot
    directLine.postActivity({
      from: { id: this.userId },
      type: 'event',
      name: 'chatbot/join',
      value: { language: window.navigator.language }
    }).subscribe(
      id => console.log('Posted activity, assigned ID', id),
      error => console.log('Error posting activity', error)
    );

    // listen to activities sent from the bot
    directLine.activity$
      .subscribe(activity => {
        console.log('received activity ', activity);
      });

  }

  private ChatbotSetup() {

    const styleOptions = this.styleOptionsSetup();

    const directLine = new DirectLine({
      secret: 'p0-7g3F6OQo.rLokJQ0_cfGLGeJgQzp-omxdNyIl5bJroSajSfOX9nc',
      webSocket: false
    });

    // https://github.com/microsoft/BotFramework-DirectLineJS
    window.WebChat.renderWebChat({
      directLine: directLine,
      userID: this.userId,
      webSpeechPonyfillFactory: window.WebChat.createBrowserWebSpeechPonyfillFactory(), // to enable speech button
      styleOptions,
      locale: 'pt-BR',
      sendTypingIndicator: true,
    }, this.botWindowElement.nativeElement);

    return directLine;
  }

  GetOrSetGuid(): string {
    const USER_ID_NAME = 'userID';
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
      botAvatarImage: 'https://azr-br-rg-bot-webui.azurewebsites.net/images/avatar/avatar-face2.png',
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
}
