import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { OrderFormCatalogs } from '../models/order-form-catalogs.model';

@Injectable({
  providedIn: 'root',
})
export class CatalogsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/catalogs`;

  getOrderFormCatalogs(): Observable<OrderFormCatalogs> {
    return this.http.get<OrderFormCatalogs>(`${this.baseUrl}/order-form`);
  }

  invalidateCatalogCache(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/cache/invalidate`, {});
  }
}
