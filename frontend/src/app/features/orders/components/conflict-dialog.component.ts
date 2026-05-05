import { CommonModule, DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Order } from '../models/order.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';

interface ComparisonRow {
  field: string;
  current: string;
  attempted: string;
  status: 'Sin cambio' | 'Diferente' | 'Requiere revisión';
}

@Component({
  selector: 'app-conflict-dialog',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './conflict-dialog.component.html',
  styleUrl: './conflict-dialog.component.css',
})
export class ConflictDialogComponent {
  readonly currentOrder = input.required<Order>();
  readonly attemptedChanges = input.required<UpdateOrderRequest>();
  readonly reload = output<void>();
  readonly close = output<void>();
  readonly copyAttemptedComment = output<void>();

  getComparisonRows(): ComparisonRow[] {
    const cur = this.currentOrder();
    const att = this.attemptedChanges();

    const fields: Array<{ field: string; current: string; attempted: string; isVersion?: boolean }> = [
      { field: 'Cliente', current: cur.customerName, attempted: att.customerName },
      { field: 'Email', current: cur.customerEmail, attempted: att.customerEmail },
      { field: 'Dirección de entrega', current: cur.deliveryAddress, attempted: att.deliveryAddress },
      { field: 'Comentario interno', current: cur.internalComment || '—', attempted: att.internalComment || '—' },
      { field: 'Estado', current: String(cur.statusId), attempted: String(att.statusId) },
      { field: 'Método de envío', current: String(cur.shippingMethodId), attempted: String(att.shippingMethodId) },
      { field: 'Versión', current: String(cur.version), attempted: String(att.version), isVersion: true },
    ];

    return fields.map((f) => ({
      field: f.field,
      current: f.current,
      attempted: f.attempted,
      status: f.isVersion
        ? 'Requiere revisión'
        : f.current === f.attempted
          ? 'Sin cambio'
          : 'Diferente',
    }));
  }
}
