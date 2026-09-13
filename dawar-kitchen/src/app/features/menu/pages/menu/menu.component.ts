import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '@core/http/api.service';
import { MenuItem } from '../../models/menu.model';
import { CartService } from '../../../checkout/services/cart.service';
import { SeoService } from '@shared/services';
import { PricePipe } from '@shared/pipes';
import { CustomDropdownComponent } from '@shared/components';

@Component({
  selector: 'app-menu-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, CustomDropdownComponent, TranslateModule, PricePipe],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="min-h-screen pt-28 pb-16 px-6 bg-[#0a0a0a]">
      <div class="max-w-7xl mx-auto">

        <!-- Breadcrumb -->
        <nav data-cy="breadcrumb" class="flex items-center gap-2 text-xs text-neutral-500 mb-8">
          <a routerLink="/" class="hover:text-white transition-colors">{{ 'nav.home' | translate }}</a>
          <span>/</span>
          <span class="text-neutral-300">{{ 'nav.menu' | translate }}</span>
        </nav>

        <!-- Mobile filter toggle -->
        <div class="lg:hidden flex items-center justify-between mb-4">
          <span class="text-sm text-neutral-400">
            {{ filteredItems.length }} {{ filteredItems.length === 1 ? ('checkout.item' | translate) : ('checkout.items' | translate) }}
          </span>
          <button (click)="filtersOpen = !filtersOpen"
                  class="flex items-center gap-2 px-4 py-2 text-sm text-white border border-white/15 rounded-xl hover:bg-white/5 transition-all">
            <iconify-icon [icon]="filtersOpen ? 'solar:close-linear' : 'solar:filter-linear'" width="16"></iconify-icon>
            {{ filtersOpen ? ('common.close' | translate) : ('menu.filters' | translate) }}
          </button>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-4 gap-8">

          <!-- ── Filters Sidebar ── -->
          <div [class.hidden]="!filtersOpen"
               class="lg:block lg:col-span-1 space-y-6 p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 h-fit">
            <h2 class="font-['Forum'] text-xl text-white mb-4 hidden lg:block">{{ 'menu.filters' | translate }}</h2>

            <!-- Category -->
            <div class="space-y-2">
              <label class="text-xs font-medium text-neutral-400 uppercase tracking-wider">{{ 'menu.categories' | translate }}</label>
              <app-custom-dropdown
                data-cy="category-filter"
                [options]="categoryOptions"
                [selectedValue]="activeCategory"
                [placeholder]="'common.selectOption' | translate"
                [icon]="'solar:tag-linear'"
                (valueSelected)="onCategorySelected($event)">
              </app-custom-dropdown>
            </div>

            <!-- Search -->
            <div class="space-y-2">
              <label class="text-xs font-medium text-neutral-400 uppercase tracking-wider">{{ 'nav.menu' | translate }}</label>
              <input type="search"
                     [(ngModel)]="searchQuery"
                     (input)="applyFilters()"
                     [placeholder]="'menu.searchPlaceholder' | translate"
                     class="nn-input" />
            </div>

            <!-- Sort -->
            <div class="space-y-2">
              <label class="text-xs font-medium text-neutral-400 uppercase tracking-wider">{{ 'menu.sortBy' | translate }}</label>
              <app-custom-dropdown
                [options]="sortOptions"
                [selectedValue]="sortByDisplay"
                [placeholder]="'common.selectOption' | translate"
                [icon]="'solar:sort-from-top-linear'"
                (valueSelected)="onSortSelected($event)">
              </app-custom-dropdown>
            </div>

            <!-- Dietary -->
            <div class="space-y-3 pt-2">
              <label class="text-xs font-medium text-neutral-400 uppercase tracking-wider block">{{ 'menu.dietary' | translate }}</label>
              <label class="flex items-center gap-3 text-sm text-neutral-300 cursor-pointer">
                <input type="checkbox" name="vegetarian" [(ngModel)]="dietary.vegetarian" (change)="applyFilters()" />
                <span>{{ 'menu.vegetarian' | translate }}</span>
              </label>
              <label class="flex items-center gap-3 text-sm text-neutral-300 cursor-pointer">
                <input type="checkbox" name="vegan" [(ngModel)]="dietary.vegan" (change)="applyFilters()" />
                <span>{{ 'menu.vegan' | translate }}</span>
              </label>
              <label class="flex items-center gap-3 text-sm text-neutral-300 cursor-pointer">
                <input type="checkbox" name="glutenFree" [(ngModel)]="dietary.glutenFree" (change)="applyFilters()" />
                <span>{{ 'menu.glutenFree' | translate }}</span>
              </label>
            </div>

            <!-- Price Range -->
            <div class="space-y-2 pt-2">
              <label class="text-xs font-medium text-neutral-400 uppercase tracking-wider block">{{ 'menu.priceRange' | translate }}</label>
              <div class="grid grid-cols-2 gap-2">
                <input type="number" name="minPrice" [(ngModel)]="minPrice" (input)="applyFilters()"
                       [placeholder]="'menu.min' | translate" class="nn-input" />
                <input type="number" name="maxPrice" [(ngModel)]="maxPrice" (input)="applyFilters()"
                       [placeholder]="'menu.max' | translate" class="nn-input" />
              </div>
            </div>
          </div>

          <!-- ── Menu Items Grid ── -->
          <div class="lg:col-span-3">

            <!-- Loading skeletons -->
            <div *ngIf="loading" class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div *ngFor="let i of [1,2,3,4]; trackBy: trackByIndex"
                   class="animate-pulse p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 h-48"></div>
            </div>

            <!-- Empty state -->
            <div *ngIf="!loading && filteredItems.length === 0"
                 class="text-center py-16 text-neutral-500">
              {{ 'menu.noItemsFound' | translate }}
            </div>

            <!-- Items grid -->
            <div *ngIf="!loading && filteredItems.length > 0"
                 class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div *ngFor="let item of filteredItems; trackBy: trackByItem"
                   data-cy="menu-item"
                   class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 flex flex-col justify-between hover:border-white/10 transition-all duration-300 group">
                <div>
                  <div class="flex justify-between items-start gap-4 mb-2">
                    <h3 data-cy="item-name"
                        class="font-['Forum'] text-xl text-white group-hover:text-[#C65A1E] transition-colors">
                      {{ item.name }}
                    </h3>
                    <span class="font-['Forum'] text-lg text-white font-medium shrink-0">
                      {{ item.price | price }}
                    </span>
                  </div>
                  <p class="text-xs text-neutral-400 font-light leading-relaxed mb-4">
                    {{ item.description }}
                  </p>
                </div>

                <div class="flex items-center justify-between mt-auto pt-4 border-t border-white/5">
                  <div class="flex gap-2 flex-wrap">
                    <span data-cy="item-category"
                          class="text-[9px] tracking-wider uppercase bg-white/5 text-neutral-400 px-2 py-0.5 rounded border border-white/10">
                      {{ item.category }}
                    </span>
                    <span *ngIf="item.isVegan"
                          class="text-[9px] tracking-wider uppercase bg-[#C65A1E]/10 text-[#C65A1E] px-2 py-0.5 rounded border border-[#C65A1E]/20">
                      {{ 'menu.vegan' | translate }}
                    </span>
                    <span *ngIf="item.isVegetarian && !item.isVegan"
                          class="text-[9px] tracking-wider uppercase bg-emerald-500/10 text-emerald-400 px-2 py-0.5 rounded border border-emerald-500/20">
                      {{ 'menu.vegetarian' | translate }}
                    </span>
                    <span *ngIf="item.isGlutenFree"
                          class="text-[9px] tracking-wider uppercase bg-blue-500/10 text-blue-400 px-2 py-0.5 rounded border border-blue-500/20">
                      {{ 'menu.glutenFree' | translate }}
                    </span>
                  </div>
                  <button (click)="addToCart(item)"
                          class="px-4 py-1.5 rounded-lg text-xs font-medium text-white bg-[#C65A1E] hover:bg-[#a84915] transition-colors flex items-center gap-2 shrink-0">
                    {{ 'menu.addToCart' | translate }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class MenuPageComponent implements OnInit {
  private readonly api       = inject(ApiService);
  private readonly cart      = inject(CartService);
  private readonly seo       = inject(SeoService);
  private readonly cdr       = inject(ChangeDetectorRef);
  private readonly translate = inject(TranslateService);

  allItems: MenuItem[]      = [];
  filteredItems: MenuItem[] = [];
  categories: string[]      = [];

  activeCategory = 'All';
  searchQuery    = '';
  sortBy         = 'newest';
  dietary        = { vegetarian: false, vegan: false, glutenFree: false };
  minPrice: number | null = null;
  maxPrice: number | null = null;
  loading      = true;
  filtersOpen  = false;

  get sortOptions(): string[] {
    return [
      this.translate.instant('menu.newestFirst'),
      this.translate.instant('menu.priceLowToHigh'),
      this.translate.instant('menu.priceHighToLow'),
      this.translate.instant('menu.nameAtoZ')
    ];
  }

  ngOnInit(): void {
    this.seo.setMenu();

    this.api.getMenu().subscribe({
      next: (items) => {
        this.allItems   = items;
        this.categories = [...new Set(items.map(i => i.category))];
        this.applyFilters();
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  get categoryOptions(): string[] {
    return ['All', ...this.categories];
  }

  applyFilters(): void {
    let items = [...this.allItems];

    if (this.activeCategory !== 'All') {
      items = items.filter(i => i.category === this.activeCategory);
    }
    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      items = items.filter(i =>
        i.name.toLowerCase().includes(q) || i.description.toLowerCase().includes(q)
      );
    }
    if (this.dietary.vegetarian) items = items.filter(i => i.isVegetarian);
    if (this.dietary.vegan)      items = items.filter(i => i.isVegan);
    if (this.dietary.glutenFree) items = items.filter(i => i.isGlutenFree);

    if (this.minPrice !== null) items = items.filter(i => i.price >= this.minPrice!);
    if (this.maxPrice !== null) items = items.filter(i => i.price <= this.maxPrice!);

    if (this.sortBy === 'price-asc')  items.sort((a, b) => a.price - b.price);
    else if (this.sortBy === 'price-desc') items.sort((a, b) => b.price - a.price);
    else if (this.sortBy === 'name')  items.sort((a, b) => a.name.localeCompare(b.name));
    else items.sort((a, b) => b.sortOrder - a.sortOrder);

    this.filteredItems = items;
  }

  trackByItem(_index: number, item: MenuItem): string { return item.id; }
  trackByString(_index: number, val: string): string  { return val; }
  trackByIndex(_index: number): number                { return _index; }

  onCategorySelected(category: string): void {
    this.activeCategory = category;
    this.applyFilters();
  }

  onSortSelected(sortOption: string): void {
    const map: { [key: string]: string } = {};
    map[this.translate.instant('menu.newestFirst')]    = 'newest';
    map[this.translate.instant('menu.priceLowToHigh')] = 'price-asc';
    map[this.translate.instant('menu.priceHighToLow')] = 'price-desc';
    map[this.translate.instant('menu.nameAtoZ')]       = 'name';
    this.sortBy = map[sortOption] || 'newest';
    this.applyFilters();
  }

  get sortByDisplay(): string {
    const map: { [key: string]: string } = {
      'newest':     this.translate.instant('menu.newestFirst'),
      'price-asc':  this.translate.instant('menu.priceLowToHigh'),
      'price-desc': this.translate.instant('menu.priceHighToLow'),
      'name':       this.translate.instant('menu.nameAtoZ')
    };
    return map[this.sortBy] || this.translate.instant('menu.newestFirst');
  }

  addToCart(item: MenuItem): void {
    this.cart.addItem({
      menuItemId: item.id,
      name:       item.name,
      price:      item.price,
      category:   item.category,
      quantity:   1
    });
  }
}
