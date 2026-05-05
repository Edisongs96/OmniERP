import { HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { CatalogsApiService } from '../services/catalogs-api.service';
import { OrdersApiService } from '../services/orders-api.service';
import { ApiError } from '../models/api-error.model';
import {
  ORDER_CONCURRENCY_CONFLICT,
  OrderConflictResponse,
} from '../models/order-conflict.model';
import { OrderFormCatalogs } from '../models/order-form-catalogs.model';
import { Order } from '../models/order.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';

interface OrderEditState {
  order: Order | null;
  catalogs: OrderFormCatalogs | null;
  loading: boolean;
  loadingCatalogs: boolean;
  saving: boolean;
  error: string | null;
  successMessage: string | null;
  conflict: OrderConflictResponse | null;
}

const initialState: OrderEditState = {
  order: null,
  catalogs: null,
  loading: false,
  loadingCatalogs: false,
  saving: false,
  error: null,
  successMessage: null,
  conflict: null,
};

@Injectable({
  providedIn: 'root',
})
export class OrderEditStore {
  private readonly ordersApi = inject(OrdersApiService);
  private readonly catalogsApi = inject(CatalogsApiService);
  private readonly state = signal<OrderEditState>(initialState);
  private readonly currentOrderId = signal<number | null>(null);

  readonly order = computed(() => this.state().order);
  readonly catalogs = computed(() => this.state().catalogs);
  readonly loading = computed(() => this.state().loading);
  readonly loadingCatalogs = computed(() => this.state().loadingCatalogs);
  readonly saving = computed(() => this.state().saving);
  readonly error = computed(() => this.state().error);
  readonly successMessage = computed(() => this.state().successMessage);
  readonly conflict = computed(() => this.state().conflict);

  loadOrder(id: number): void {
    if (id <= 0) {
      this.patch({ error: 'El identificador del pedido no es valido.' });
      return;
    }

    this.currentOrderId.set(id);
    this.patch({
      loading: true,
      error: null,
      successMessage: null,
      conflict: null,
    });

    this.ordersApi
      .getOrderById(id)
      .pipe(finalize(() => this.patch({ loading: false })))
      .subscribe({
        next: (order) => this.patch({ order }),
        error: (error: unknown) =>
          this.patch({ error: this.toLoadOrderMessage(error) }),
      });
  }

  loadCatalogs(): void {
    this.patch({
      loadingCatalogs: true,
      error: null,
      successMessage: null,
    });

    this.catalogsApi
      .getOrderFormCatalogs()
      .pipe(finalize(() => this.patch({ loadingCatalogs: false })))
      .subscribe({
        next: (catalogs) => this.patch({ catalogs }),
        error: () =>
          this.patch({
            error: 'No fue posible cargar los catalogos del formulario.',
          }),
      });
  }

  saveOrder(request: UpdateOrderRequest): void {
    const id = this.currentOrderId() ?? this.state().order?.id;

    if (!id) {
      this.patch({ error: 'No hay un pedido cargado para guardar.' });
      return;
    }

    this.patch({
      saving: true,
      error: null,
      successMessage: null,
      conflict: null,
    });

    this.ordersApi
      .updateOrder(id, request)
      .pipe(finalize(() => this.patch({ saving: false })))
      .subscribe({
        next: (order) =>
          this.patch({
            order,
            successMessage: 'Pedido actualizado correctamente.',
            conflict: null,
          }),
        error: (error: unknown) => this.handleSaveError(error, request),
      });
  }

  clearMessages(): void {
    this.patch({
      error: null,
      successMessage: null,
    });
  }

  clearConflict(): void {
    this.patch({ conflict: null });
  }

  reloadCurrentOrder(): void {
    const id = this.currentOrderId();

    if (id) {
      this.loadOrder(id);
    }
  }

  private handleSaveError(
    error: unknown,
    attemptedChanges: UpdateOrderRequest,
  ): void {
    if (error instanceof HttpErrorResponse && error.status === 409) {
      const conflict = this.toConflictResponse(error, attemptedChanges);
      this.patch({
        conflict,
        error: conflict.message || 'El pedido fue actualizado por otro agente.',
      });
      return;
    }

    if (error instanceof HttpErrorResponse && error.status === 400) {
      this.patch({ error: this.getApiMessage(error.error) });
      return;
    }

    this.patch({
      error: 'Ocurrio un error inesperado. Intenta nuevamente.',
    });
  }

  private toConflictResponse(
    error: HttpErrorResponse,
    attemptedChanges: UpdateOrderRequest,
  ): OrderConflictResponse {
    const response = error.error as Partial<OrderConflictResponse> | null;

    return {
      code: response?.code ?? ORDER_CONCURRENCY_CONFLICT,
      message:
        response?.message ??
        'El pedido fue actualizado por otro agente. Revisa la version actual.',
      currentOrder: response?.currentOrder ?? this.state().order!,
      attemptedChanges: response?.attemptedChanges ?? attemptedChanges,
    };
  }

  private toLoadOrderMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse && error.status === 404) {
      return 'No se encontro el pedido solicitado.';
    }

    return 'No fue posible cargar el pedido.';
  }

  private getApiMessage(error: unknown): string {
    const apiError = error as Partial<ApiError> | null;
    return apiError?.message || 'La solicitud no es valida.';
  }

  private patch(patch: Partial<OrderEditState>): void {
    this.state.update((current) => ({
      ...current,
      ...patch,
    }));
  }
}
