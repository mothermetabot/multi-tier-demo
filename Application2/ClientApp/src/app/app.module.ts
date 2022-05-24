import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { REAL_TIME_SERVICE } from './constants/di-tokens.constants';
import { SignalrRealTimeService } from './services/signalr.real-time.service';

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
  providers: [
    {
      provide: REAL_TIME_SERVICE,
      useClass: SignalrRealTimeService,
    }  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
