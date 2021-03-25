
import { Component, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { LanguageComponent } from './language.component';
import { ExecutionComponent } from './execution.component';

@Component({
    selector: "testbed",
    template: `
            <languageSelector (onLanguageChange)="languageChanged($event)"></languageSelector>
            <execution></execution>        
    `,
    providers: [LanguageComponent, ExecutionComponent]
})

export class TestBedComponent {

    @ViewChild(ExecutionComponent)
    private _executionComponent: ExecutionComponent;

    currentLanguage: string;

    languageChanged(arg) {
        console.log('(Parent) --> Language changed to ' + arg);
        console.log(this._executionComponent);
        this._executionComponent.changeLanguage(arg);
    }

}