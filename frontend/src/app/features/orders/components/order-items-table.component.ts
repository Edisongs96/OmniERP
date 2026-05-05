import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { OrderItem } from '../models/order-item.model';

@Component({
  selector: 'app-order-items-table',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './order-items-table.component.html',
  styleUrl: './order-items-table.component.css',
})
export class OrderItemsTableComponent {
  readonly items = input.required<readonly OrderItem[]>();

  total(item: OrderItem): number {
    return item.quantity * item.unitPrice;
  }
}
