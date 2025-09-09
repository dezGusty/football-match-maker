import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stat-selector',
  imports: [CommonModule],
  templateUrl: './stat-selector.html',
  styleUrl: './stat-selector.css'
})
export class StatSelector {
  @Input() speed: number = 2;
  @Input() stamina: number = 2;
  @Input() errors: number = 2;

  @Output() speedChange = new EventEmitter<number>();
  @Output() staminaChange = new EventEmitter<number>();
  @Output() errorsChange = new EventEmitter<number>();

  updateSpeed(value: number) {
    this.speed = value;
    this.speedChange.emit(value);
  }

  updateStamina(value: number) {
    this.stamina = value;
    this.staminaChange.emit(value);
  }

  updateErrors(value: number) {
    this.errors = value;
    this.errorsChange.emit(value);
  }
}
