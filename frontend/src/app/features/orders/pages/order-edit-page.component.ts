import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConflictDialogComponent } from '../components/conflict-dialog.component';
import { OrderFormComponent } from '../components/order-form.component';
import { OrderItemsTableComponent } from '../components/order-items-table.component';
import { UpdateOrderRequest } from '../models/update-order-request.model';
import { OrderEditStore } from '../state/order-edit.store';

@Component({
  selector: 'app-order-edit-page',
  standalone: true,
  imports: [
    CommonModule,
    ConflictDialogComponent,
    OrderFormComponent,
    OrderItemsTableComponent,
  ],
  templateUrl: './order-edit-page.component.html',
  styleUrl: './order-edit-page.component.css',
})
export class OrderEditPageComponent implements OnInit {
  protected readonly store = inject(OrderEditStore);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    const routeId = Number(this.route.snapshot.paramMap.get('id'));
    const orderId = Number.isFinite(routeId) && routeId > 0 ? routeId : 1001;

    this.store.loadCatalogs();
    this.store.loadOrder(orderId);
  }

  onSave(request: UpdateOrderRequest): void {
    this.store.saveOrder(request);
  }

  onReloadCurrentOrder(): void {
    this.store.clearConflict();
    this.store.reloadCurrentOrder();
  }
}
