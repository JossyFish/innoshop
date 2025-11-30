'use client';
import { useCallback, useLayoutEffect, useRef, useState } from "react";
import { GetProducts } from "./Middlewares/productMiddleware";
import { GetProductsFilters } from "./Models/GetProductsFilters";
import { Product } from "./Models/Product";
import { ProductModel } from "./Components/ProductModel/ProductModel";
import styles from  "./main.module.css";
import { FiltersModel } from "./Components/FiltersModel/FiltersModel";
import { useRouter } from "next/navigation";

export default function Home() {
  const [filters, setFilters] = useState(GetProductsFilters.create());
  const [products, setProducts] = useState<Product[]>([]);
  const [productsCount, setProductsCount] = useState(0);
  const [message, setMessage] = useState<string | null>(null); 
  const loadMoreRef = useRef<HTMLDivElement>(null); 
  const observerRef = useRef<IntersectionObserver | null>(null);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [searchValue, setSearchValue] = useState('');
  const [showFiltersModel, setShowFiltersModel] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const router = useRouter();

  const fetchProducts = useCallback(async (isLoadMore = false) => {
    if (loading) return;
    
    setLoading(true);

    const currentPageNumber = filters.pageNumber || 1;
    const currentPage = isLoadMore ? currentPageNumber + 1 : 1;
    
    const requestFilters = GetProductsFilters.updatePageNumber(filters, currentPage);
    const response = await GetProducts(requestFilters);
    
    if (response.success && response.data) {
      if (response.data.products && Array.isArray(response.data.products)) {
          const newProducts = response.data.products;
          
          if (isLoadMore) {
              setProducts(prev => {
                  // Удаляем дубликаты по ID
                  const existingIds = new Set(prev.map(p => p.id));
                  const uniqueNewProducts = newProducts.filter(product => !existingIds.has(product.id));
                  return [...prev, ...uniqueNewProducts];
              });
          } else {
              setProducts(newProducts);
          }
          
          const responsePageNumber = response.data.pageNumber || 1;
          setFilters(prev => GetProductsFilters.updatePageNumber(prev, responsePageNumber));
          setCurrentPage(responsePageNumber);
          setProductsCount(response.data.totalCount);
          setHasMore(responsePageNumber < response.data.totalPages);
          setMessage(null);
      } else {
          setMessage('Invalid server request');
      }
    } else {
        if (response.error) {
            setMessage(`${response.error.title}: ${response.error.detail}`);
        } else {
            setMessage('Server error');
        }
        
        if (!isLoadMore) {
            setProducts([]);
        }
    }
    setLoading(false);

  }, [filters, loading]);

  useLayoutEffect(() => {
    fetchProducts(false);
  }, [filters.name, filters.minPrice, filters.maxPrice, filters.currency, filters.minQuantity, filters.minCreatedAt, filters.maxCreatedAt]); 

  useLayoutEffect(() => {
    if (loading || !hasMore) return;

    observerRef.current = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          fetchProducts(true);
        }
      },
      { threshold: 0.1 }
    );

    if (loadMoreRef.current) {
      observerRef.current.observe(loadMoreRef.current);
    }

    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, [loading, hasMore, fetchProducts]);

  // Создаем уникальные ключи с учетом страницы и индекса
  const getProductKey = (product: Product, index: number) => {
    return `${product.id}-${currentPage}-${index}`;
  };

  const SearchKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      setFilters(prev => ({
        ...GetProductsFilters.updateName(prev, searchValue),
        pageNumber: 1
      }));
    }
  };

  const handleSearchChange = (value: string) => {
    setSearchValue(value);
  };

  const handleFiltersChange = (newFilters: GetProductsFilters) => {
    setFilters({
      ...newFilters,
      pageNumber: 1 
    });
    setShowFiltersModel(false);
  };

  const handleProductClick = (productId: string) => {
    router.push(`/product/${productId}`);
  }

  return (
    <div>
      <header className={styles.header}>
        <input  
          value={searchValue} 
          onChange={(e) => handleSearchChange(e.target.value)}
          onKeyDown={SearchKeyDown}
          placeholder="Found"
          className={styles.search}>
        </input>
      </header>
      <div className={styles.content}> 
        <button className={styles.filters} onClick={() => setShowFiltersModel(true)}>
          <img className={styles.filtersImg} src="/static/filters.svg" alt="Filters"/>
          Filters
        </button>
        {message && (
          <div> {message} </div> )}
        {!message && products.length > 0 && (
          <>
            <p className={styles.foundedScore}>{productsCount} products found</p>
            <div className={styles.products}>
              {products.map((product, index) => (
                <ProductModel 
                  key={getProductKey(product, index)}
                  product={product} 
                  onProductClick={handleProductClick}
                />
              ))}
              <div ref={loadMoreRef} style={{ height: '1px' }} />
            </div>
          </>
        )}
        
        {!message && products.length === 0 && !loading && (
          <div className={styles.noProducts}>Products not found</div>
        )}
      </div>
      {showFiltersModel && (
        <FiltersModel filters={filters} onClose={() => setShowFiltersModel(false)} onFiltersChange={handleFiltersChange} />
      )}
    </div>
  );
}