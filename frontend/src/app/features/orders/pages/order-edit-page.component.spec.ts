import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { CatalogMetadata } from '../models/order-form-catalogs.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';
import { OrderConflictResponse } from '../models/order-conflict.model';
import { mockCatalogs } from '../testing/order-test-data';
import { OrderEditStore } from '../state/order-edit.store';
import { OrderEditPageComponent } from './order-edit-page.component';

describe('OrderEditPageComponent', () => {
  let fixture: ComponentFixture<OrderEditPageComponent>;
  const store = {
    order: signal(null),
    catalogs: signal(null),
    catalogMetadata: signal<CatalogMetadata | null>(null),
    loading: signal(false),
    loadingCatalogs: signal(false),
    saving: signal(false),
    error: signal<string | null>(null),
    successMessage: signal<string | null>(null),
    conflict: signal<OrderConflictResponse | null>(null),
    clipboardMessage: signal<string | null>(null),
    lastAttemptedChanges: signal<UpdateOrderRequest | null>(null),
    loadOrder: jasmine.createSpy('loadOrder'),
    loadCatalogs: jasmine.createSpy('loadCatalogs'),
    saveOrder: jasmine.createSpy('saveOrder'),
    clearConflict: jasmine.createSpy('clearConflict'),
    reloadCurrentOrder: jasmine.createSpy('reloadCurrentOrder'),
    copyAttemptedCommentToClipboard: jasmine.createSpy('copyAttemptedCommentToClipboard'),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderEditPageComponent],
      providers: [
        { provide: OrderEditStore, useValue: store },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({ id: '1001' }),
            },
          },
        },
      ],
    }).compileComponents();

    store.loadOrder.calls.reset();
    store.loadCatalogs.calls.reset();
    fixture = TestBed.createComponent(OrderEditPageComponent);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
    expect(store.loadCatalogs).toHaveBeenCalled();
    expect(store.loadOrder).toHaveBeenCalledWith(1001);
  });

  it('should display catalog metadata source and duration', () => {
    store.catalogMetadata.set(mockCatalogs.metadata);
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Catálogos desde caché');
    expect(text).toContain('12ms');
  });
});
