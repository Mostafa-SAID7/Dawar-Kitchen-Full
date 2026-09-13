import { ChangeDetectionStrategy, Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageService } from '@shared/services';
import { CATEGORIES_DATA } from '../../../../../data/category.data';
import { RevealDirective, ImageOptimizationDirective } from '@shared/directives';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective, ImageOptimizationDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryComponent {
  categories = CATEGORIES_DATA;
  readonly currentLang$ = inject(LanguageService).getCurrentLanguage();

  trackByIndex(_index: number): number { return _index; }
}
