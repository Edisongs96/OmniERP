import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OrderFormComponent } from './order-form.component';
import { mockCatalogs, mockOrder } from '../testing/order-test-data';

describe('OrderFormComponent', () => {
  let fixture: ComponentFixture<OrderFormComponent>;
  let component: OrderFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderFormComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('order', mockOrder);
    fixture.componentRef.setInput('catalogs', mockCatalogs);
    fixture.componentRef.setInput('saving', false);
    fixture.detectChanges();
  });

  it('should validate required fields', () => {
    component.form.controls.customerName.setValue('');
    component.form.controls.customerEmail.setValue('');
    component.form.controls.deliveryAddress.setValue('');
    component.form.controls.statusId.setValue(0);
    component.form.controls.shippingMethodId.setValue(0);

    expect(component.form.invalid).toBeTrue();
  });

  it('should emit save with version', () => {
    spyOn(component.save, 'emit');

    component.form.setValue({
      customerName: 'Cliente Demo',
      customerEmail: 'cliente.demo@omnierp.local',
      deliveryAddress: 'Nueva direccion',
      internalComment: 'Nuevo comentario',
      statusId: 2,
      shippingMethodId: 1,
    });

    component.submit();

    expect(component.save.emit).toHaveBeenCalledWith({
      customerName: 'Cliente Demo',
      customerEmail: 'cliente.demo@omnierp.local',
      deliveryAddress: 'Nueva direccion',
      internalComment: 'Nuevo comentario',
      statusId: 2,
      shippingMethodId: 1,
      version: mockOrder.version,
      updatedBy: 'frontend.agent',
    });
  });
});
