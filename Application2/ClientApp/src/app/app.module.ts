import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { SignalrRealTimeService } from './services/signalr.real-time.service';
import { HubConnection } from '@microsoft/signalr';
import { hubConnectionFactory } from './services/real-time-service.provider';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
    ])
  ],
  providers: [ SignalrRealTimeService, {
    provide: HubConnection,
    useFactory: hubConnectionFactory
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
