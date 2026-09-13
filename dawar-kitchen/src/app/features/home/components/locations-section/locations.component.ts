import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RevealDirective, ImageOptimizationDirective } from '@shared/directives';

@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective, ImageOptimizationDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './locations.component.html',
  styleUrls: ['./locations.component.css']
})
export class LocationsComponent {}
