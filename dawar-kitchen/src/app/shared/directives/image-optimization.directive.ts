import { Directive, Input, ElementRef, OnChanges, SimpleChanges, OnInit } from '@angular/core';

/**
 * Image optimization directive (v2)
 * - Lazy loading for below-fold images
 * - fetchpriority for LCP candidates
 * - Responsive srcset + sizes
 * - Aspect ratio to prevent CLS
 * - WebP/AVIF support
 */
@Directive({
  selector: '[appImageOptimization]',
  standalone: true,
})
export class ImageOptimizationDirective implements OnInit, OnChanges {
  @Input() appImageOptimization!: string; // Primary image path (WebP or modern format)
  @Input() alt: string = '';
  @Input() width?: number;
  @Input() height?: number;
  @Input() aspectRatio?: string; // CSS aspect-ratio (e.g., '16/9')
  @Input() priority: boolean = false; // LCP candidate: priority + fetchpriority="high"
  @Input() srcset?: string; // Responsive srcset (e.g., "image-640w.webp 640w, image-1024w.webp 1024w")
  @Input() sizes?: string; // Responsive sizes (e.g., "(max-width: 640px) 100vw, (max-width: 1024px) 80vw, 1024px")

  constructor(private el: ElementRef<HTMLImageElement>) {}

  ngOnInit(): void {
    this.applyOptimizations();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['appImageOptimization'] || changes['priority'] || changes['srcset'] || changes['sizes']) {
      this.applyOptimizations();
    }
  }

  private applyOptimizations(): void {
    const img = this.el.nativeElement;

    // Primary src
    img.src = this.appImageOptimization;
    img.alt = this.alt;

    // Responsive images
    if (this.srcset) {
      img.srcset = this.srcset;
    }
    if (this.sizes) {
      img.sizes = this.sizes;
    }

    // Loading strategy
    if (this.priority) {
      // LCP candidate: eager load with high priority
      img.loading = 'eager';
      img.fetchPriority = 'high';
      img.removeAttribute('decoding');
    } else {
      // Below-fold: lazy load
      img.loading = 'lazy';
      img.fetchPriority = 'low';
      img.decoding = 'async';
    }

    // Dimensions & CLS prevention
    if (this.width) img.width = this.width;
    if (this.height) img.height = this.height;

    // Aspect ratio for CLS prevention when dimensions not set
    if (this.aspectRatio && !this.width && !this.height) {
      img.style.aspectRatio = this.aspectRatio;
    }
  }
}
