
import { Component, Input } from "@angular/core";
import { Router, Route } from '@angular/router';

@Component({
    selector: "navbar",
    template: `
<div class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a asp-controller="Home" asp-action="Index" class="navbar-brand">ELTB</a>
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li *ngFor="let route of routes" >
                        <a [routerLinkActive]="['active']" [routerLink]="[route.path]">{{route.data.name}}</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>
    `
})

export class NavbarComponent {

    routes: Route[];

    constructor(router: Router) {
        this.routes = router.config.filter(r => r.path && r.path.indexOf('*') < 0);
    }

}