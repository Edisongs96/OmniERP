import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Order } from '../models/order.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';

@Injectable({
  providedIn: 'root',
})
export class OrdersApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/orders`;

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`);
  }

  updateOrder(id: number, request: UpdateOrderRequest): Observable<Order> {
    return this.http.put<Order>(`${this.baseUrl}/${id}`, request);
  }
}
