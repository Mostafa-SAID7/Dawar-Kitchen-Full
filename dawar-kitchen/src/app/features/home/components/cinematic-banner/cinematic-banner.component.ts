import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-cinematic-banner',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './cinematic-banner.component.html',
  styleUrls: ['./cinematic-banner.component.css']
})
export class CinematicBannerComponent {}
