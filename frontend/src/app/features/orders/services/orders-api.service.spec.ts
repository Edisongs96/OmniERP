import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { mockOrder, mockUpdateRequest } from '../testing/order-test-data';
import { OrdersApiService } from './orders-api.service';

describe('OrdersApiService', () => {
  let service: OrdersApiService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        OrdersApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(OrdersApiService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should call get order endpoint', () => {
    service.getOrderById(1001).subscribe((order) => {
      expect(order.id).toBe(1001);
    });

    const request = httpTesting.expectOne(`${environment.apiBaseUrl}/orders/1001`);
    expect(request.request.method).toBe('GET');
    request.flush(mockOrder);
  });

  it('should call update order endpoint', () => {
    service.updateOrder(1001, mockUpdateRequest).subscribe((order) => {
      expect(order.version).toBe(2);
    });

    const request = httpTesting.expectOne(`${environment.apiBaseUrl}/orders/1001`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(mockUpdateRequest);
    request.flush({ ...mockOrder, version: 2 });
  });
});
