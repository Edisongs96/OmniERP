import { ComponentFixture, TestBed } from '@angular/core/testing';
import { mockOrder, mockUpdateRequest } from '../testing/order-test-data';
import { ConflictDialogComponent } from './conflict-dialog.component';

const currentOrderWithConflict = {
  ...mockOrder,
  version: 2,
  deliveryAddress: 'Direccion actual del sistema',
};

const attemptedWithDifferentAddress = {
  ...mockUpdateRequest,
  deliveryAddress: 'Direccion intentada',
  version: 1,
};

describe('ConflictDialogComponent', () => {
  let fixture: ComponentFixture<ConflictDialogComponent>;
  let component: ConflictDialogComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConflictDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ConflictDialogComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('currentOrder', currentOrderWithConflict);
    fixture.componentRef.setInput('attemptedChanges', attemptedWithDifferentAddress);
    fixture.detectChanges();
  });

  it('should render conflict title and message', () => {
    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Conflicto de actualización detectado');
    expect(text).toContain('ORDER_CONCURRENCY_CONFLICT');
  });

  it('should display different fields between currentOrder and attemptedChanges', () => {
    const rows = component.getComparisonRows();
    const addressRow = rows.find((r) => r.field === 'Dirección de entrega');

    expect(addressRow).toBeDefined();
    expect(addressRow!.status).toBe('Diferente');

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Diferente');
  });

  it('should emit reload when reload button is clicked', () => {
    spyOn(component.reload, 'emit');

    const btn = fixture.nativeElement.querySelector(
      'button.btn-primary',
    ) as HTMLButtonElement;
    btn.click();

    expect(component.reload.emit).toHaveBeenCalled();
  });

  it('should emit close when close button is clicked', () => {
    spyOn(component.close, 'emit');

    const buttons = fixture.nativeElement.querySelectorAll(
      'button.btn-secondary',
    ) as NodeListOf<HTMLButtonElement>;
    const closeBtn = Array.from(buttons).find((b) =>
      b.textContent?.trim().startsWith('Cerrar'),
    );

    expect(closeBtn).toBeDefined();
    closeBtn!.click();

    expect(component.close.emit).toHaveBeenCalled();
  });

  it('should emit copyAttemptedComment when copy button is clicked', () => {
    spyOn(component.copyAttemptedComment, 'emit');

    const buttons = fixture.nativeElement.querySelectorAll(
      'button.btn-secondary',
    ) as NodeListOf<HTMLButtonElement>;
    const copyBtn = Array.from(buttons).find((b) =>
      b.textContent?.includes('Copiar comentario'),
    );

    expect(copyBtn).toBeDefined();
    copyBtn!.click();

    expect(component.copyAttemptedComment.emit).toHaveBeenCalled();
  });
});
