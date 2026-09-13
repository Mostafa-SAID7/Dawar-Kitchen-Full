import { Validators, ValidatorFn } from '@angular/forms';
import { VALIDATION, REGEX } from '../constants';
import { CustomValidators } from './custom.validators';

export class AppValidators {
  static get required(): ValidatorFn {
    return Validators.required;
  }

  static get email(): ValidatorFn[] {
    return [Validators.required, Validators.pattern(REGEX.EMAIL)];
  }

  static get password(): ValidatorFn[] {
    return [
      Validators.required,
      Validators.minLength(VALIDATION.PASSWORD.MIN_LENGTH),
      Validators.maxLength(VALIDATION.PASSWORD.MAX_LENGTH)
    ];
  }

  /** Use for name / full-name fields — avoids clash with Function.name */
  static get fullName(): ValidatorFn[] {
    return [
      Validators.required,
      Validators.minLength(VALIDATION.NAME.MIN_LENGTH),
      Validators.maxLength(VALIDATION.NAME.MAX_LENGTH)
    ];
  }

  static get phone(): ValidatorFn[] {
    return [
      Validators.required,
      Validators.minLength(VALIDATION.PHONE.MIN_LENGTH),
      Validators.maxLength(VALIDATION.PHONE.MAX_LENGTH),
      Validators.pattern(REGEX.PHONE)
    ];
  }

  static passwordMatch(passwordField: string = 'password', confirmField: string = 'confirmPassword'): ValidatorFn {
    return CustomValidators.passwordMatch(passwordField, confirmField);
  }

  static get futureDate(): ValidatorFn[] {
    return [Validators.required, CustomValidators.futureDate()];
  }
}
