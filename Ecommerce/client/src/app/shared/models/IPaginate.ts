export interface IPaginate<T> {
  data: T[] ;
  count: number;
  pageIndex: number;
  pageSize: number;
}
