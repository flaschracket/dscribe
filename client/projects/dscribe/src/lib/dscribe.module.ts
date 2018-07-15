import {NgModule} from '@angular/core';
import {DscribeComponent} from './dscribe.component';
import {ListContainerComponent} from './list/list-container/list-container.component';
import {ListComponent} from './list/list/list.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {DscribeInterceptor} from './common/dscribe-interceptor';
import {
	MatAutocompleteModule,
	MatButtonModule,
	MatCardModule,
	MatCheckboxModule,
	MatDatepickerModule,
	MatDialogModule,
	MatFormFieldModule,
	MatIconModule,
	MatInputModule,
	MatMenuModule,
	MatNativeDateModule,
	MatPaginatorModule,
	MatProgressSpinnerModule,
	MatSelectModule,
	MatSortModule,
	MatTableModule
} from '@angular/material';
import {CommonModule} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {AddNEditComponent} from './add-n-edit/add-n-edit.component';
import {ListAddNEditDialogComponent} from './list/list-add-n-edit-dialog/list-add-n-edit-dialog.component';
import {PropertyEditorComponent} from './property-editors/property-editor.component';
import {BoolEditorComponent} from './property-editors/bool-editor.component';
import {DateEditorComponent} from './property-editors/date-editor.component';
import {DatetimeEditorComponent} from './property-editors/datetime-editor.component';
import {EntityAutoCompleteComponent} from './property-editors/entity-auto-complete.component';
import {EntityListEditorComponent} from './property-editors/entity-list-editor.component';
import {EntitySelectComponent} from './property-editors/entity-select.component';
import {TextEditorComponent} from './property-editors/text-editor.component';
import {NumberEditorComponent} from './property-editors/number-editor.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {ListDeleteDialogComponent} from './list/list-delete-dialog/list-delete-dialog.component';
import {FilterNodeComponent} from './filtering/components/filter-node/filter-node.component';
import {ArithmeticFilterNodeComponent} from './filtering/components/arithmetic-filter-node/arithmetic-filter-node.component';
import {ComparisonFilterNodeComponent} from './filtering/components/comparison-filter-node/comparison-filter-node.component';
import {ConstantFilterNodeComponent} from './filtering/components/constant-filter-node/constant-filter-node.component';
import {LambdaFilterNodeComponent} from './filtering/components/lambda-filter-node/lambda-filter-node.component';
import {LogicalFilterNodeComponent} from './filtering/components/logical-filter-node/logical-filter-node.component';
import {NavigationListFilterNodeComponent} from './filtering/components/navigation-list-filter-node/navigation-list-filter-node.component';
import {PropertyFilterNodeComponent} from './filtering/components/property-filter-node/property-filter-node.component';
import {FilterTreeManipulator} from './filtering/models/filter-tree-manipulator';
import {NavigationComponent} from './navigation/navigation.component';
import {HomePageComponent} from './home-page/home-page.component';
import {MetadataManagementComponent} from './Administration/metadata-management/metadata-management.component';
import {RouterModule} from '@angular/router';
import {DSCRIBE_ROUTES} from './dscribe-routes';
import {EntityGeneralUsageNamePipe} from './Administration/helpers/entity-general-usage-name.pipe';
import {AddNEditEntityComponent} from './Administration/add-n-edit-entity/add-n-edit-entity.component';
import {AddNEditPropertyComponent} from './Administration/add-n-edit-property/add-n-edit-property.component';
import {KeysPipe} from './helpers/keys.pipe';
import { ConfirmationDialogComponent } from './common/confirmation-dialog/confirmation-dialog.component';

@NgModule({
	imports: [
		BrowserAnimationsModule,
		CommonModule,
		FormsModule,
		HttpClientModule,
		MatAutocompleteModule,
		MatButtonModule,
		MatCardModule,
		MatDatepickerModule,
		MatDialogModule,
		MatIconModule,
		MatInputModule,
		MatFormFieldModule,
		MatMenuModule,
		MatNativeDateModule,
		MatPaginatorModule,
		MatSortModule,
		MatSelectModule,
		MatTableModule,
		MatCheckboxModule,
		MatProgressSpinnerModule,
		ReactiveFormsModule,
		RouterModule.forRoot(DSCRIBE_ROUTES)
	],
	declarations: [
		DscribeComponent,
		AddNEditComponent,
		ListContainerComponent,
		ListComponent,
		ListAddNEditDialogComponent,
		ListDeleteDialogComponent,
		PropertyEditorComponent,
		BoolEditorComponent,
		DateEditorComponent,
		DatetimeEditorComponent,
		EntityAutoCompleteComponent,
		EntityListEditorComponent,
		EntitySelectComponent,
		NumberEditorComponent,
		TextEditorComponent,
		FilterNodeComponent,
		ArithmeticFilterNodeComponent,
		ComparisonFilterNodeComponent,
		ConstantFilterNodeComponent,
		LambdaFilterNodeComponent,
		LogicalFilterNodeComponent,
		NavigationListFilterNodeComponent,
		PropertyFilterNodeComponent,
		NavigationComponent,
		HomePageComponent,
		MetadataManagementComponent,
		EntityGeneralUsageNamePipe,
		AddNEditEntityComponent,
		AddNEditPropertyComponent,
		KeysPipe,
		ConfirmationDialogComponent
	],
	exports: [DscribeComponent, ListComponent],
	providers: [
		{provide: HTTP_INTERCEPTORS, useClass: DscribeInterceptor, multi: true},
		FilterTreeManipulator
	],
	entryComponents: [
		ListAddNEditDialogComponent,
		ListDeleteDialogComponent,
		AddNEditEntityComponent,
		AddNEditPropertyComponent,
		ConfirmationDialogComponent
	]
})
export class DscribeModule {
}
