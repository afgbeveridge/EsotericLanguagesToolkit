
import { Component, Input } from "@angular/core";
import { SummariserPipe } from '../shared/summariser.pipe'

@Component({
    selector: "expandableSummary",
    template: `
        {{ content | summariser:limit }} <a href='#' *ngIf="action == 'less' || (content && content.length > limit)" (click)="toggle()"  [text]="action"></a>
    `
})

export class ExpandableSummaryComponent {

    private _content: string;
    private _limit: number;
    private _expandLabel = 'more';
    private _contractLabel = 'less';

    get content() {
        return this._content;
    }

    get limit() {
        return this._limit;
    }

    @Input() set content(newValue: string) {
        this._content = newValue;
        this.action = this._expandLabel;
        this.limit = this._initialLimit;
    }
    @Input() set limit(newValue: number) {
        if (!this._initialLimit || this._initialLimit < 0)
            this._initialLimit = newValue;
        this._limit = newValue;
    }
    action: string;
    private _initialLimit: number = -1;

    toggle() {
        if (this.action === this._expandLabel)
            this.limit = this.content && this.content.length > this.limit ? this.content.length + 1 : this.limit;
        else
            this.limit = this._initialLimit;
        this.action = this.action == this._expandLabel ? this._contractLabel : this._expandLabel;
    }
}