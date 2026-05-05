import { CatalogItem } from './catalog.model';

export interface CatalogMetadata {
  source: string;
  durationMs: number;
  cacheKey: string;
}

export interface OrderFormCatalogs {
  orderStatuses: readonly CatalogItem[];
  shippingMethods: readonly CatalogItem[];
  metadata: CatalogMetadata;
}
