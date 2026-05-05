import { OrderItem } from './order-item.model';

export interface Order {
  id: number;
  customerName: string;
  customerEmail: string;
  deliveryAddress: string;
  internalComment: string;
  statusId: number;
  shippingMethodId: number;
  version: number;
  updatedAt: string;
  updatedBy: string;
  items: readonly OrderItem[];
}
