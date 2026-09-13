import { Component, EventEmitter, Output, Input, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClickOutsideDirective } from '../../directives';

@Component({
  selector: 'app-custom-dropdown',
  standalone: true,
  imports: [CommonModule, ClickOutsideDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './custom-dropdown.component.html',
  styleUrls: ['./custom-dropdown.component.css']
})
export class CustomDropdownComponent {
  @Input() options: string[] = [];
  @Input() selectedValue: string = '';
  @Input() placeholder: string = 'Select option';
  @Input() icon: string = 'solar:alt-arrow-down-linear';
  @Output() valueSelected = new EventEmitter<string>();
  
  isOpen = false;

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  selectOption(option: string) {
    this.selectedValue = option;
    this.valueSelected.emit(option);
    this.isOpen = false;
  }

  getDisplayValue(): string {
    return this.selectedValue || this.placeholder;
  }
}
