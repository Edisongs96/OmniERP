export interface UpdateOrderRequest {
  customerName: string;
  customerEmail: string;
  deliveryAddress: string;
  internalComment: string;
  statusId: number;
  shippingMethodId: number;
  version: number;
  updatedBy: string;
}
