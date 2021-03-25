import {Injectable} from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/common/http';
import { Observer, NextObserver } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import {$WebSocket} from 'angular2-websocket/angular2-websocket';
import { SourceBundle } from './sourcebundle.interface';
import { LanguageDescription } from './languageDescription.interface';

@Injectable()
export class EsolangService {

    private static _interruptSequence = '\t';
    private static _targetUrl = "api/EsotericLanguage/";
    private _channel: $WebSocket;

    constructor(private _http: Http) {
        console.log('built ELS - ok' + this._http);
    }

    supportedLanguages() {
        return this
            ._http
            .get(EsolangService._targetUrl + "SupportedLanguages")
            .map(res => <LanguageDescription[]> res.json());
    }

    execute(bundle: SourceBundle, handler: NextObserver<any>, interrupt: () => void) {
        this.close();
        this._channel = new $WebSocket("ws://" + window.location.host + "/" + EsolangService._targetUrl + "execute");
        this._channel.connect();
        this.send('|' + bundle.language + '|' + bundle.source);
        this._channel
            .getDataStream()
            .subscribe(m => {
                var received = m['data'];
                if (!received || received[0] != EsolangService._interruptSequence) {
                    console.log(received);
                    handler.next(received);
                }
                else
                    interrupt();
            },
            e => console.log('WS error: ' + e),
            () => {
                console.log('Seems like the subject is exhausted');
                handler.complete();
            }
        );
    }

    send(text: string) {
        this._channel.send(text).subscribe();
    }

    close() {
        if (this._channel) {
            this._channel.close(true);
            this._channel = null;
            console.log('Closed ws');
        }
    }

}
