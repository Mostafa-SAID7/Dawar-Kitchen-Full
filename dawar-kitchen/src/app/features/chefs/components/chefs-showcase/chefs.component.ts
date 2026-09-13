import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '@core/http/api.service';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';
import { ImageOptimizationDirective } from '@shared/directives/image-optimization.directive';
import { Chef, ChefView } from '@features/chefs/models/chef.model';

const CHEF_IMAGES = [
  'assets/chefs/chef-mahmoud.jpg',
  'assets/chefs/chef-fatima.jpg',
];


@Component({
  selector: 'app-chefs',
  standalone: true,
  imports: [CommonModule, TranslateModule, RevealDirective, ImageOptimizationDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './chefs.component.html',
  styleUrls: ['./chefs.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChefsComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  chefs: ChefView[] = [];
  loading = true;
  error = false;

  ngOnInit(): void {
    this.api.getChefs().subscribe({
      next: (chefs: Chef[]) => {
        this.chefs = chefs.map((chef, i) => ({
          name: chef.name,
          role: chef.title,
          image: chef.imageUrl || CHEF_IMAGES[i % CHEF_IMAGES.length],
          bio: chef.bio,
          specialty: chef.specialty,
          initial: chef.name.charAt(0)
        }));
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = true;
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  revealDelay(i: number): number { return i * 120; }
  trackByChef(_index: number, chef: ChefView): string { return chef.name; }
  trackByIndex(_index: number): number { return _index; }
}
