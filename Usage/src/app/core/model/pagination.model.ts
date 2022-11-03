export class PaginationModel {

  allItemsLength = 0;
  selectItemsPerPage: number[] = [50, 100, 200];
  pageSize = this.selectItemsPerPage[1];
  pageIndex = 1;
}
