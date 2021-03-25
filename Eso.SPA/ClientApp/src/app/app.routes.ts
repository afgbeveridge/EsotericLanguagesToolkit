import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { TestBedComponent }  from './home/testbed.component';

const routes: Routes = [
    { path: 'home', component: TestBedComponent, data: { name: 'Home' } },
    { path: 'about', component: TestBedComponent, data: { name: 'About' } },
    { path: '', component: TestBedComponent },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
