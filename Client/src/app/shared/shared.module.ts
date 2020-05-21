import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MaterialModule } from './material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { LocationService } from './utilities/location.service';

@NgModule({
  declarations: [
    
  ],
  providers: [
    LocationService
  ],
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  exports: [
    MaterialModule,
    FormsModule,
    FlexLayoutModule,
    ReactiveFormsModule,
  ]
})
export class SharedModule { }
