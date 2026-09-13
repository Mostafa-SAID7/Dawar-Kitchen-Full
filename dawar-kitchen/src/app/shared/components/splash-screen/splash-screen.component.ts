import {
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ChangeDetectionStrategy,
  HostBinding
} from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * SplashScreenComponent
 * Renders the full-screen loading splash that shows while Angular bootstraps.
 * Controlled by the `visible` input: when it becomes false the splash
 * fades out and removes itself from the DOM.
 */
@Component({
  selector: 'app-splash-screen',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="splash-logo">
      <div class="splash-mark">
        <img src="assets/Dawar-logo.png" alt="Dawar Kitchen" width="64" height="64">
      </div>
      <div>
        <div class="splash-name">Dawar Kitchen</div>
        <div class="splash-sub">Authentic Egyptian &amp; Syrian Cuisine</div>
      </div>
    </div>
    <div class="splash-bar"></div>
  `,
  styles: [`
    :host {
      position: fixed;
      inset: 0;
      z-index: 9999;
      background: #0a0a0a;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 28px;
      transition: opacity 0.45s cubic-bezier(0.4, 0, 0.2, 1);
    }

    :host.fade-out {
      opacity: 0;
      pointer-events: none;
    }

    /* Logo group */
    .splash-logo {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 14px;
      animation: splash-rise 0.55s cubic-bezier(0.34, 1.56, 0.64, 1) both;
    }

    /* Logo mark */
    .splash-mark {
      width: 64px;
      height: 64px;
      border-radius: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: transparent;
      box-shadow: 0 0 40px rgba(198, 90, 30, 0.35);
      animation: splash-pulse 2.4s ease-in-out infinite;
    }
    .splash-mark img {
      width: 100%;
      height: 100%;
      border-radius: 16px;
      object-fit: contain;
    }

    /* Brand text */
    .splash-name {
      font-family: 'Forum', serif;
      font-size: 22px;
      color: rgba(255, 255, 255, 0.92);
      letter-spacing: 0.04em;
      font-weight: 400;
      text-align: center;
    }
    .splash-sub {
      font-family: 'Open Sans', system-ui, sans-serif;
      font-size: 10px;
      color: #C65A1E;
      letter-spacing: 0.22em;
      text-transform: uppercase;
      margin-top: -6px;
      text-align: center;
    }

    /* Shimmer bar */
    .splash-bar {
      width: 120px;
      height: 2px;
      background: rgba(255, 255, 255, 0.06);
      border-radius: 1px;
      overflow: hidden;
      animation: splash-rise 0.55s 0.1s cubic-bezier(0.34, 1.56, 0.64, 1) both;
    }
    .splash-bar::after {
      content: '';
      display: block;
      width: 40%;
      height: 100%;
      background: linear-gradient(90deg, transparent, #C65A1E, transparent);
      border-radius: 1px;
      animation: splash-slide 1.4s ease-in-out infinite;
    }

    /* Keyframes */
    @keyframes splash-rise {
      from { opacity: 0; transform: translateY(16px); }
      to   { opacity: 1; transform: translateY(0); }
    }
    @keyframes splash-pulse {
      0%, 100% { box-shadow: 0 0 30px rgba(198, 90, 30, 0.25); }
      50%       { box-shadow: 0 0 50px rgba(198, 90, 30, 0.55); }
    }
    @keyframes splash-slide {
      0%   { transform: translateX(-250%); }
      100% { transform: translateX(600%); }
    }
  `]
})
export class SplashScreenComponent implements OnChanges {
  /** Set to false to trigger the fade-out + self-removal */
  @Input() visible = true;

  @HostBinding('class.fade-out') isFadingOut = false;
  @HostBinding('attr.aria-hidden') ariaHidden = true;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && !this.visible) {
      this.isFadingOut = true;
    }
  }
}
