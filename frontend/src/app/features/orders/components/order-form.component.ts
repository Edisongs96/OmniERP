import { CommonModule, DatePipe } from '@angular/common';
import { Component, computed, effect, inject, input, output } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { OrderFormCatalogs } from '../models/order-form-catalogs.model';
import { Order } from '../models/order.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, DatePipe, ReactiveFormsModule],
  templateUrl: './order-form.component.html',
  styleUrl: './order-form.component.css',
})
export class OrderFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly order = input.required<Order>();
  readonly catalogs = input<OrderFormCatalogs | null>(null);
  readonly saving = input(false);
  readonly save = output<UpdateOrderRequest>();

  readonly orderStatuses = computed(() => this.catalogs()?.orderStatuses ?? []);
  readonly shippingMethods = computed(() => this.catalogs()?.shippingMethods ?? []);

  readonly form = this.formBuilder.group({
    customerName: ['', Validators.required],
    customerEmail: ['', [Validators.required, Validators.email]],
    deliveryAddress: ['', Validators.required],
    internalComment: [''],
    statusId: [0, [Validators.required, Validators.min(1)]],
    shippingMethodId: [0, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    effect(() => {
      const order = this.order();

      this.form.reset({
        customerName: order.customerName,
        customerEmail: order.customerEmail,
        deliveryAddress: order.deliveryAddress,
        internalComment: order.internalComment,
        statusId: order.statusId,
        shippingMethodId: order.shippingMethodId,
      });
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const order = this.order();
    const value = this.form.getRawValue();

    this.save.emit({
      customerName: value.customerName,
      customerEmail: value.customerEmail,
      deliveryAddress: value.deliveryAddress,
      internalComment: value.internalComment,
      statusId: value.statusId,
      shippingMethodId: value.shippingMethodId,
      version: order.version,
      updatedBy: 'frontend.agent',
    });
  }

  hasError(controlName: keyof typeof this.form.controls, errorCode: string): boolean {
    const control = this.form.controls[controlName];
    return control.hasError(errorCode) && (control.dirty || control.touched);
  }
}
