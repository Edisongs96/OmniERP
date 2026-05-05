import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ORDER_CONCURRENCY_CONFLICT } from '../models/order-conflict.model';
import { mockOrder, mockUpdateRequest } from '../testing/order-test-data';
import { OrderEditStore } from './order-edit.store';

describe('OrderEditStore', () => {
  let store: OrderEditStore;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [OrderEditStore],
    });

    store = TestBed.inject(OrderEditStore);
    httpMock = TestBed.inject(HttpTestingController);

    // Prime with a loaded order so saveOrder has an ID
    store.loadOrder(1001);
    const req = httpMock.expectOne('http://localhost:5000/api/v1/orders/1001');
    req.flush(mockOrder);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should set conflict when API returns 409', () => {
    const conflictBody = {
      code: ORDER_CONCURRENCY_CONFLICT,
      message: 'El pedido fue actualizado por otro agente.',
      currentOrder: { ...mockOrder, version: 2 },
      attemptedChanges: mockUpdateRequest,
    };

    store.saveOrder(mockUpdateRequest);

    const req = httpMock.expectOne('http://localhost:5000/api/v1/orders/1001');
    req.flush(conflictBody, { status: 409, statusText: 'Conflict' });

    expect(store.conflict()).not.toBeNull();
    expect(store.conflict()!.code).toBe(ORDER_CONCURRENCY_CONFLICT);
    expect(store.lastAttemptedChanges()).toEqual(mockUpdateRequest);
  });
});
