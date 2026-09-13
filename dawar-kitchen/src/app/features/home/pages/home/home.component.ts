import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { HeroComponent } from '../../components/hero-section/hero.component';
import { CinematicBannerComponent } from '../../components/cinematic-banner/cinematic-banner.component';
import { CategoryComponent } from '../../../menu/components/category-filter/category.component';
import { MenuComponent } from '../../../menu/components/menu-display/menu.component';
import { SeoService } from '@shared/services';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    HeroComponent,
    CinematicBannerComponent,
    CategoryComponent,
    MenuComponent,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {
  private readonly seo = inject(SeoService);

  ngOnInit(): void {
    this.seo.setHome();
  }
}
