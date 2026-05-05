import { CommonModule, DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { OrderConflictResponse } from '../models/order-conflict.model';

@Component({
  selector: 'app-conflict-dialog',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './conflict-dialog.component.html',
  styleUrl: './conflict-dialog.component.css',
})
export class ConflictDialogComponent {
  readonly conflict = input.required<OrderConflictResponse>();
  readonly reload = output<void>();
  readonly close = output<void>();
}
