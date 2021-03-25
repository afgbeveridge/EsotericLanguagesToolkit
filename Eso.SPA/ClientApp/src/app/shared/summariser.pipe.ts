
import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'summariser'
})
export class SummariserPipe implements PipeTransform {

    transform(value: string, lim: number) {
        var limit = lim <= 0 ? 40 : lim;
        return !value || value.length < limit ? 
            value :
            value.substring(0, limit) + '...';
    }

}