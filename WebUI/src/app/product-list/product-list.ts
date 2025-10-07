import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Product {
  productID: number;
  articleNumber: string;
  articleName: string;
  description: string;
  category: string;
  tags: string[];
}

@Component({
  selector: 'app-product-list',
  imports: [NgbModule, CommonModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductList implements OnInit {
  private http = inject(HttpClient);
  public products = signal<Product[]>([]);
  public categories = signal<string[]>([]);
  public selectedCategory = signal<string>('');

  async ngOnInit(): Promise<void> {
    await this.loadCategories();
    await this.loadProducts();
  }

  private async loadCategories(): Promise<void> {
    const result = await firstValueFrom(this.http.get<string[]>(
      `${environment.apiBaseUrl}/products/categories`));
    this.categories.set(result);
  }

  private async loadProducts(): Promise<void> {
    const category = this.selectedCategory();
    const url = category
      ? `${environment.apiBaseUrl}/products?category=${encodeURIComponent(category)}`
      : `${environment.apiBaseUrl}/products`;

    const result = await firstValueFrom(this.http.get<Product[]>(url));
    this.products.set(result);
  }

  async onCategoryChange(category: string): Promise<void> {
    this.selectedCategory.set(category);
    await this.loadProducts();
  }
}
