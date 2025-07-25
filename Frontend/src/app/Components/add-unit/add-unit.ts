import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Unit } from '../../Services/unit';


enum UnitType {
  Apartment = 0,
  Chalet = 1,
  Villa = 2,
}

@Component({
  selector: 'app-add-unit',
  templateUrl: './add-unit.html',
  styleUrls: ['./add-unit.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class AddUnit implements OnInit {
  unitForm: FormGroup;
  contractFile: File | null = null;
  selectedAmenityIds: number[] = [];
  unitFiles: FileList | null = null;



  unitTypes = [
    { value: UnitType.Apartment, label: 'Apartment' },
    { value: UnitType.Chalet, label: 'Chalet' },
    { value: UnitType.Villa, label: 'Villa' }
  ];

  amenities = [
    { id: 1, name: 'Wifi' },
    { id: 2, name: 'Pool' },
    { id: 3, name: 'Air Conditioning' },
  ];

  constructor(private fb: FormBuilder, private http: HttpClient, private unitService: Unit) {

    this.unitForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      unitType: ['', Validators.required],
      bedrooms: [null, [Validators.required, Validators.min(1)]],
      bathrooms: [null, [Validators.required, Validators.min(1)]],
      sleeps: [null, [Validators.required, Validators.min(1)]],
      distanceToSea: [null, [Validators.required, Validators.min(0)]],
      basePricePerNight: [null, [Validators.required, Validators.min(0.01)]],
      address: ['', Validators.required],
      villageName: ['', Validators.required],

      contractDocument: [null, Validators.required],

      unitImages:[null, Validators.required]

    });
  }

  ngOnInit(): void {

    this.unitService.GetUnitById(3).subscribe({
      next: (unit) => {
        console.log('Fetched unit:', unit);
      },
      error: (err) => console.error('Error fetching unit:', err)
    });
  }

  onSubmit() {

    if (this.unitForm.invalid || !this.contractFile) {

      this.unitForm.markAllAsTouched();
      console.error('Please fill out all required fields and attach a contract document.');
      return;
    }
    console.log('Submitted form data:', this.unitForm.value);
    this.submitForm();
  }

  onContractFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.contractFile = input.files[0];

      this.unitForm.get('contractDocument')?.setValue(this.contractFile);
    } else {
      this.contractFile = null;
      this.unitForm.get('contractDocument')?.setValue(null);
    }
  }

  onUnitFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.unitFiles = input.files;

      this.unitForm.get('unitImages')?.setValue(this.unitFiles);
    } else {
      this.unitFiles = null;
      this.unitForm.get('unitImages')?.setValue(null);
    }
  }


  onAmenityChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    const id = parseInt(checkbox.value, 10);
    if (checkbox.checked) {
      this.selectedAmenityIds.push(id);
    } else {
      this.selectedAmenityIds = this.selectedAmenityIds.filter(a => a !== id);
    }
  }

  submitForm() {
    const formData = new FormData();
    Object.keys(this.unitForm.value).forEach(key => {
        if (key !== 'contractDocument') {
            formData.append(key, this.unitForm.get(key)?.value);
        }
    });

    if (this.contractFile) {
      formData.append('ContractDocument', this.contractFile, this.contractFile.name);
    }
    if (this.unitFiles) {
      for (let i = 0; i < this.unitFiles?.length; i++)
      {
        formData.append('unitImages', this.unitFiles[i], this.unitFiles[i].name)
      }
      console.log(this.unitFiles);
    }
    if(this.amenities)

    this.selectedAmenityIds.forEach(id => {
      formData.append('AmenityIds', id.toString());
      console.log('Selected Amenity ID:', id);
    });

    console.log('Form Data');
    console.log(formData);
    this.unitService.AddUnit(formData).subscribe({
      next: res => {
        console.log('unit added successfully', res);

        this.unitForm.reset();
        this.contractFile = null;
        this.selectedAmenityIds = [];
      },
      error: err => {
        console.error('error on adding unit', err);

      }
    });
  }
}
