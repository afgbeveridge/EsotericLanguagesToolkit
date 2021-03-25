import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { AppComponent }  from './app.component';
import { NavbarComponent } from "./shared/navbar.component";
import { routing } from './app.routes';
import { TestBedComponent } from './home/testbed.component';
import { LanguageComponent } from './home/language.component';
import { ExecutionComponent } from './home/execution.component';
import { ExpandableSummaryComponent } from './shared/expandable-summary.component';
import { SummariserPipe } from './shared/summariser.pipe'
import { $WebSocket } from 'angular2-websocket/angular2-websocket';


@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpClient,
        routing
    ],
    declarations: [
        AppComponent, NavbarComponent, TestBedComponent, LanguageComponent, ExecutionComponent, ExpandableSummaryComponent, SummariserPipe
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
    
}
