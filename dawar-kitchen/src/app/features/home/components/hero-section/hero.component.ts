import { ChangeDetectionStrategy, Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [RouterModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './hero.component.html',
  styleUrls: ['./hero.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeroComponent {}
