import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ORDER_CONCURRENCY_CONFLICT } from '../models/order-conflict.model';
import { mockOrder, mockUpdateRequest } from '../testing/order-test-data';
import { ConflictDialogComponent } from './conflict-dialog.component';

describe('ConflictDialogComponent', () => {
  let fixture: ComponentFixture<ConflictDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConflictDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ConflictDialogComponent);
    fixture.componentRef.setInput('conflict', {
      code: ORDER_CONCURRENCY_CONFLICT,
      message: 'El pedido fue actualizado por otro agente.',
      currentOrder: {
        ...mockOrder,
        version: 2,
        deliveryAddress: 'Direccion actual',
      },
      attemptedChanges: mockUpdateRequest,
    });
    fixture.detectChanges();
  });

  it('should display currentOrder and attemptedChanges', () => {
    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Direccion actual');
    expect(text).toContain(mockUpdateRequest.deliveryAddress);
    expect(text).toContain('ORDER_CONCURRENCY_CONFLICT');
  });
});
