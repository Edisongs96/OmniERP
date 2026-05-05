import { Order } from './order.model';
import { UpdateOrderRequest } from './update-order-request.model';

export const ORDER_CONCURRENCY_CONFLICT = 'ORDER_CONCURRENCY_CONFLICT' as const;

export type OrderConflictCode = typeof ORDER_CONCURRENCY_CONFLICT;

export interface OrderConflictResponse {
  code: OrderConflictCode | string;
  message: string;
  currentOrder: Order;
  attemptedChanges: UpdateOrderRequest;
}
