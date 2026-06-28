import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IPagination, Pagination } from '../shared/models/pagination';
import { IType } from '../shared/models/productType';
import { map } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { IProduct } from '../shared/models/product';
import { of } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  products: IProduct[] = [];
  brands: IBrand[] = [];
  types: IType[] = [];
  pagination = new Pagination();
  shopParams = new ShopParams();
  productCache = new Map();

  constructor(private http: HttpClient) { }

getProducts(useCache: boolean) {
  // إعادة الكاش لو طلب المستخدم
  if (useCache === false) {
    this.productCache = new Map();
  }

  // توليد مفتاح الكاش بشكل آمن
  const cacheKey = Object.values(this.shopParams)
    .map(v => (v != null ? v.toString() : ''))  // أي undefined تتحول لسلسلة فارغة
    .join('-');

  // استخدام الكاش لو موجود
  if (useCache && this.productCache.size > 0 && this.productCache.has(cacheKey)) {
    this.pagination.data = this.productCache.get(cacheKey);
    return of(this.pagination);
  }

  // إنشاء الـ HttpParams بشكل آمن
  let params = new HttpParams();

  if (this.shopParams.brandId != null && this.shopParams.brandId !== 0) {
    params = params.append('brandId', this.shopParams.brandId.toString());
  }

  if (this.shopParams.typeId != null && this.shopParams.typeId !== 0) {
    params = params.append('typeId', this.shopParams.typeId.toString());
  }

  if (this.shopParams.search) {
    params = params.append('search', this.shopParams.search);
  }

  // قيم default آمنة
  params = params.append('sort', this.shopParams.sort ?? 'nameAsc');
  params = params.append('pageIndex', (this.shopParams.pageNumber ?? 1).toString());
  params = params.append('pageSize', (this.shopParams.pageSize ?? 6).toString());

  // طلب البيانات من السيرفر
  return this.http.get<IPagination>(this.baseUrl + 'products', { observe: 'response', params })
    .pipe(
      map(response => {
        // حفظ البيانات في الكاش
        this.productCache.set(cacheKey, response.body.data);

        // تحديث pagination
        this.pagination = response.body;

        return this.pagination;
      })
    );
}


  setShopParams(params: ShopParams) {
    this.shopParams = params;
  }

  getShopParams() {
    return this.shopParams;
  }

  getProduct(id: number) {
    let product: IProduct;
    this.productCache.forEach((products: IProduct[]) => {
      console.log(product);
      product = products.find(p => p.id === id);
    })

    if (product) {
      return of(product);
    }

    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  getBrands() {
    if (this.brands.length > 0) {
      return of(this.brands);
    }
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands').pipe(
      map(response => {
        this.brands = response;
        return response;
      })
    )
  }

  getTypes() {
    if (this.types.length > 0) {
      return of(this.types);
    }
    return this.http.get<IType[]>(this.baseUrl + 'products/types').pipe(
      map(response => {
        this.types = response;
        return response;
      })
    )
  }
}
