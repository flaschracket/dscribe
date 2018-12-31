import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatPaginator, MatTableDataSource } from '@angular/material';
import { EntityTypeHistoryModel } from '../../models/history/entity-type-history-model';
import { SnackBarService } from 'src/lib/common/notifications/snackbar.service';
import { HistoryService } from 'src/lib/lob-tools/history-service';

@Component({
	selector: 'dscribe-entity-history',
	templateUrl: './entity-history.component.html',
	styleUrls: ['./entity-history.component.css']
})
export class EntityHistoryComponent implements OnInit {

	private entityHistories: EntityTypeHistoryModel[] = [];
	private isLoading = false;
	displayedEntityTypeColumns = ['action', 'name', 'usage', 'singular', 'plural', 'code', 'displayName', 'ActionDate'];
	entityTypesDataSource = new MatTableDataSource<EntityTypeHistoryModel>(this.entityHistories);

	@ViewChild('entityTypesPaginator') entityTypesPaginator: MatPaginator;

	constructor(
		private dialogRef: MatDialogRef<EntityHistoryComponent>,
		@Inject(MAT_DIALOG_DATA) public data: EntityTypeHistoryModel,
		private snackbarService: SnackBarService,
		private historyService: HistoryService) { }

	ngOnInit() {
		this.entityTypesDataSource.paginator = this.entityTypesPaginator;
		this.getEntityHistory();
	}

	setActionIcon(data: EntityTypeHistoryModel) {
		switch (data.Action) {
			case 'addEntityType':
				return 'add';
			case 'editEntityType':
				return 'edit';
			case 'deleteEntityType':
				return 'delete';
			default: break;
		}
	}

	setActionColor(data: EntityTypeHistoryModel) {
		switch (data.Action) {
			case 'addEntityType':
				return { 'color': 'green' };
			case 'editEntityType':
				return { 'color': 'accent' };
			case 'deleteEntityType':
				return { 'color': 'red' };
			default: return {};
		}
	}

	getEntityHistory() {
		this.isLoading = true;
		this.entityHistories = [];
		this.historyService.getEntityTypeHistory(this.data).subscribe(
			(res: EntityTypeHistoryModel[]) => {
				this.isLoading = false;
				this.entityTypesDataSource.data = this.entityHistories = res;
			}, (error: HttpErrorResponse) => {
				this.snackbarService.open(error.statusText);
				this.isLoading = false;
			}
		);
	}

	getEntityTypeUsageName(id: number) {
		return this.data.basicInfo.EntityTypeGeneralUsageCategories.find(x => x.Id === id)!.Name;
	}

	cancel() {
		this.dialogRef.close();
	}

}
