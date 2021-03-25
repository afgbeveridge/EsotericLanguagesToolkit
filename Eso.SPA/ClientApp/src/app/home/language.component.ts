
import { Component, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { ExpandableSummaryComponent } from '../shared/expandable-summary.component';
import { EsolangService } from './esolang.service';
import { LanguageDescription } from './languageDescription.interface';

@Component({
    selector: "languageSelector",
    template: `
            <div class="row">
                <div class="col-xs-3" *ngFor="let language of languages">
                    <button class="btn btn-primary" (click)="languageChanged($event)">
                        {{language.name}}
                    </button>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <h3>Summary</h3>
                </div>
                <div class="col-xs-12">
                    <expandableSummary [content]="currentLanguage?.summary" [limit]="100"></expandableSummary>
                </div>
                <div class="col-xs-12">
                    <div *ngIf="currentLanguage?.detailsUrl">
                        <a [href]="currentLanguage.detailsUrl" target="_blank">Link</a>
                    <div>
                    <div *ngIf="!(currentLanguage.detailsUrl)">
                        No details Url provided
                    <div>
                </div>
            </div>     
    `,
    providers: [EsolangService]
})

export class LanguageComponent {
    @ViewChild(ExpandableSummaryComponent)
    private _summary: ExpandableSummaryComponent;
    @Output() onLanguageChange = new EventEmitter<string>();
    languages: LanguageDescription[];
    currentLanguage: LanguageDescription;

    constructor(private _esolangService: EsolangService) {
        console.log('built TBC, call service');
        this._esolangService.supportedLanguages().subscribe(t => {
            console.log(t);
            this.languages = t;
            this.currentLanguage = t[0];
        });
    }

    languageChanged(event) {
        var language = event.currentTarget.innerText;
        console.log('Language changed to ' + language);
        this.currentLanguage = this.languages.find(l => l.name == language);
        this.onLanguageChange.emit(this.currentLanguage.name);
    }

    hasTargetUrl() {
        return this.currentLanguage && this.currentLanguage.detailsUrl;
    }
}