import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '@core/http/api.service';
import { CartService } from '@features/checkout/services/cart.service';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';
import { MenuItem, MenuItemView } from '@features/menu/models/menu.model';


@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, RouterModule, RevealDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MenuComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  readonly cart = inject(CartService);

  allItems: MenuItemView[] = [];
  filteredItems: MenuItemView[] = [];
  categories: string[] = [];
  activeCategory = 'All';
  loading = true;
  error = false;
  addedId: string | null = null;

  ngOnInit(): void {
    this.api.getMenu().subscribe({
      next: (items: MenuItem[]) => {
        this.allItems = items.map(item => ({
          id: item.id,
          name: item.name,
          price: item.price,
          priceFormatted: `£${item.price.toFixed(2)}`,
          description: item.description,
          category: item.category,
          isVegetarian: item.isVegetarian,
          isVegan: item.isVegan
        }));
        const unique = [...new Set(this.allItems.map(i => i.category))];
        this.categories = ['All', ...unique];
        this.filteredItems = this.allItems;
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

  setCategory(cat: string): void {
    this.activeCategory = cat;
    this.filteredItems = cat === 'All'
      ? this.allItems
      : this.allItems.filter(i => i.category === cat);
  }

  addToCart(item: MenuItemView): void {
    this.cart.addItem({
      menuItemId: item.id,
      name: item.name,
      price: item.price,
      category: item.category,
      quantity: 1
    });
    this.addedId = item.id;
    setTimeout(() => {
      if (this.addedId === item.id) this.addedId = null;
      this.cdr.markForCheck();
    }, 1500);
  }

  itemCount(id: string): number {
    return this.cart.items().find(i => i.menuItemId === id)?.quantity ?? 0;
  }

  trackByItem(_index: number, item: MenuItemView): string { return item.id; }
  trackByCategory(_index: number, cat: string): string { return cat; }
  trackByIndex(_index: number): number { return _index; }
}
