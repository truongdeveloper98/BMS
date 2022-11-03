import { Inject, Injectable } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { PaginationModel } from '../core/model/pagination.model';

@Injectable({
    providedIn: 'any',
})

export class PaginationService {

    private paginationModel: PaginationModel;

    get page(): number {
        return this.paginationModel.pageIndex;
    }

    get selectItemsPerPage(): number[] {
        return this.paginationModel.selectItemsPerPage;
    }

    get pageCount(): number {
        return this.paginationModel.pageSize;
    }

    get pageSize(): number {
        return this.paginationModel.pageSize;
    }

    constructor(
    ) {
        this.paginationModel = new PaginationModel();
    }

    change(pageEvent: PageEvent) {
        this.paginationModel.pageIndex = pageEvent.pageIndex + 1;
        this.paginationModel.pageSize = pageEvent.pageSize;
        this.paginationModel.allItemsLength = pageEvent.length;
    }

    setIndex(idx: number){
        this.paginationModel.pageIndex = idx
    }

}
