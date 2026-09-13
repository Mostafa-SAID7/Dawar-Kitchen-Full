import { ChangeDetectionStrategy, Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CATEGORIES_DATA } from '../../../../../data/category.data';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryComponent {
  categories = CATEGORIES_DATA;

  trackByIndex(_index: number): number { return _index; }
}
