import { ChangeDetectionStrategy, Component, Input, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RevealDirective } from '../../directives/scroll-reveal.directive';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, RevealDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AboutComponent {
  @Input() standalone = false;

  trackByIndex(_index: number): number { return _index; }
  features = [
    { icon: 'solar:flame-linear', title: 'Traditional Recipes', description: 'Authentic Syrian and Egyptian recipes celebrating food heritage.' },
    { icon: 'solar:leaf-linear', title: 'Fresh Ingredients', description: 'Finest local produce sourced and prepared fresh daily.' },
    { icon: 'solar:star-linear', title: 'Authentic Flavors', description: 'Generations of culinary wisdom in every dish.' },
    { icon: 'solar:cup-hot-linear', title: 'Fair Employment', description: 'Dignified work empowering Syrian and Egyptian women.' }
  ];
}
