import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { OrderEditStore } from '../state/order-edit.store';
import { OrderEditPageComponent } from './order-edit-page.component';

describe('OrderEditPageComponent', () => {
  let fixture: ComponentFixture<OrderEditPageComponent>;
  const store = {
    order: signal(null),
    catalogs: signal(null),
    loading: signal(false),
    loadingCatalogs: signal(false),
    saving: signal(false),
    error: signal(null),
    successMessage: signal(null),
    conflict: signal(null),
    loadOrder: jasmine.createSpy('loadOrder'),
    loadCatalogs: jasmine.createSpy('loadCatalogs'),
    saveOrder: jasmine.createSpy('saveOrder'),
    clearConflict: jasmine.createSpy('clearConflict'),
    reloadCurrentOrder: jasmine.createSpy('reloadCurrentOrder'),
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
});
