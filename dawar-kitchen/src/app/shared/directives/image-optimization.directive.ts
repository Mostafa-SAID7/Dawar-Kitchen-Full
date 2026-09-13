import { Directive, Input, ElementRef, OnChanges, SimpleChanges } from '@angular/core';

/**
 * Image optimization directive
 * Adds lazy loading, responsive images, WebP support
 */
@Directive({
  selector: '[appImageOptimization]',
  standalone: true,
})
export class ImageOptimizationDirective implements OnChanges {
  @Input() appImageOptimization!: string; // Original image path
  @Input() alt: string = '';
  @Input() width?: number;
  @Input() height?: number;
  @Input() priority: boolean = false; // Set true for above-the-fold images

  constructor(private el: ElementRef<HTMLImageElement>) {}

  ngOnChanges(changes: SimpleChanges): void {
    const img = this.el.nativeElement;

    if (changes['appImageOptimization'] || changes['priority'] || changes['alt']) {
      if (this.priority) {
        // Priority images: load immediately
        img.removeAttribute('loading');
        img.src = this.appImageOptimization;
      } else {
        // Non-priority: lazy load
        img.loading = 'lazy';
        img.src = this.appImageOptimization;
      }
      img.alt = this.alt;
    }

    if (changes['width'] && this.width) img.width = this.width;
    if (changes['height'] && this.height) img.height = this.height;
  }
}
