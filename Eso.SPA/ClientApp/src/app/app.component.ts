import { Component } from '@angular/core';

@Component({
    selector: 'app-shell',
    template: `
        <navbar></navbar>
        <div class="container body-content">
            <div>
                <router-outlet></router-outlet>     
            </div>
            <hr />
            <footer>
                <p>&copy; 2016 - ELTB</p>
            </footer>
        </div>
    `
})
export class AppComponent {

}
