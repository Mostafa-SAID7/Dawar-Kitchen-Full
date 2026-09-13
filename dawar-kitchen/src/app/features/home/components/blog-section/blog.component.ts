import { ChangeDetectionStrategy, Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { BLOG_POSTS_DATA } from '../../../data/blog.data';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';
import { ImageOptimizationDirective } from '@shared/directives/image-optimization.directive';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective, ImageOptimizationDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BlogComponent {
  posts = BLOG_POSTS_DATA;

  trackByIndex(_index: number): number { return _index; }
}
