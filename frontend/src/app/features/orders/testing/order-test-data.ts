import { OrderFormCatalogs } from '../models/order-form-catalogs.model';
import { Order } from '../models/order.model';
import { UpdateOrderRequest } from '../models/update-order-request.model';

export const mockOrder: Order = {
  id: 1001,
  customerName: 'Cliente Demo',
  customerEmail: 'cliente.demo@omnierp.local',
  deliveryAddress: 'Calle 10 # 20-30, Medellin',
  internalComment: 'Pedido prioritario',
  statusId: 1,
  shippingMethodId: 2,
  version: 1,
  updatedAt: '2026-05-05T10:00:00Z',
  updatedBy: 'system',
  items: [
    {
      id: 1,
      orderId: 1001,
      productSku: 'SKU-001',
      productName: 'Producto Demo',
      quantity: 2,
      unitPrice: 50000,
    },
  ],
};

export const mockCatalogs: OrderFormCatalogs = {
  orderStatuses: [
    { id: 1, name: 'Pendiente' },
    { id: 2, name: 'En preparacion' },
  ],
  shippingMethods: [
    { id: 1, name: 'Recogida en tienda' },
    { id: 2, name: 'Envio estandar' },
  ],
  metadata: {
    source: 'cache',
    durationMs: 12,
    cacheKey: 'order-form-catalogs:v1',
  },
};

export const mockUpdateRequest: UpdateOrderRequest = {
  customerName: 'Cliente Demo',
  customerEmail: 'cliente.demo@omnierp.local',
  deliveryAddress: 'Nueva direccion',
  internalComment: 'Nuevo comentario',
  statusId: 2,
  shippingMethodId: 1,
  version: 1,
  updatedBy: 'frontend.agent',
};
