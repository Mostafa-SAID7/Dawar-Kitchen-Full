import { ChangeDetectionStrategy, Component, Input, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';
import { ImageOptimizationDirective } from '@shared/directives/image-optimization.directive';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective, ImageOptimizationDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AboutComponent {
  @Input() standalone = false;

  trackByIndex(_index: number): number { return _index; }
  features = [
    { icon: 'solar:flame-linear', titleKey: 'about.recipesTitle', descKey: 'about.recipesDesc' },
    { icon: 'solar:leaf-linear', titleKey: 'about.ingredientsTitle', descKey: 'about.ingredientsDesc' },
    { icon: 'solar:star-linear', titleKey: 'about.flavorsTitle', descKey: 'about.flavorsDesc' },
    { icon: 'solar:cup-hot-linear', titleKey: 'about.fairWorkTitle', descKey: 'about.fairWorkDesc' }
  ];
}
