import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { mockCatalogs } from '../testing/order-test-data';
import { CatalogsApiService } from './catalogs-api.service';

describe('CatalogsApiService', () => {
  let service: CatalogsApiService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CatalogsApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(CatalogsApiService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should call get catalogs endpoint', () => {
    service.getOrderFormCatalogs().subscribe((catalogs) => {
      expect(catalogs.metadata.source).toBe('cache');
    });

    const request = httpTesting.expectOne(
      `${environment.apiBaseUrl}/catalogs/order-form`,
    );
    expect(request.request.method).toBe('GET');
    request.flush(mockCatalogs);
  });

  it('should call invalidate cache endpoint', () => {
    service.invalidateCatalogCache().subscribe((response) => {
      expect(response.message).toContain('invalidated');
    });

    const request = httpTesting.expectOne(
      `${environment.apiBaseUrl}/catalogs/cache/invalidate`,
    );
    expect(request.request.method).toBe('POST');
    request.flush({ message: 'Order form catalogs cache invalidated successfully.' });
  });
});
