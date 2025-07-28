import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { AuthService } from '../../Services/auth.service';
import { Unit } from '../../Services/unit';
import { IUnit } from '../../Models/iunit';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  imports: [NgxSpinnerModule, CommonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  email: string = '';
  username: string = '';
  userRole: string = '';
  isOwner: boolean = false;
  ownerUnits: IUnit[] = [];

  constructor(
    private authService: AuthenticationService,
    private authService2: AuthService,
    private unitService: Unit,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Add debug info
    this.authService.logTokenInfo();

    // Get user role
    this.userRole = this.authService2.getCurrentUserRole() || '';
    this.isOwner = this.userRole === 'Owner';

    this.authService.getProfile().subscribe({
      next: (res) => {
        console.log('Profile loaded successfully:', res);
        this.email = res.email;
        this.username = res.username;

        // Load owner's units if user is owner
        if (this.isOwner) {
          this.loadOwnerUnits();
        }
      },
      error: (e) => {
        console.error('Profile loading failed:', e);
        console.log('Response status:', e.status);
        console.log('Response message:', e.message);
      },
    });
  }

  loadOwnerUnits(): void {
    this.spinner.show();
    this.unitService.GetMyUnits().subscribe({
      next: (units) => {
        this.ownerUnits = units;
        console.log('Owner units loaded:', units);
        this.spinner.hide();
      },
      error: (error) => {
        console.error('Error loading owner units:', error);
        this.spinner.hide();
        Swal.fire({
          title: 'Error',
          text: 'Failed to load your units',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      },
    });
  }

  deleteUnit(unitId: number): void {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        this.spinner.show();
        this.unitService.DeleteUnit(unitId).subscribe({
          next: (response) => {
            this.spinner.hide();
            Swal.fire(
              'Deleted!',
              'Your unit has been deleted.',
              'success'
            );
            // Reload units after deletion
            this.loadOwnerUnits();
          },
          error: (error) => {
            this.spinner.hide();
            console.error('Error deleting unit:', error);
            Swal.fire(
              'Error!',
              'Failed to delete unit.',
              'error'
            );
          },
        });
      }
    });
  }

  editUnit(unitId: number): void {
    // Navigate to edit unit page or open edit modal
    this.router.navigate(['/edit-unit', unitId]);
  }

  viewUnitDetails(unitId: number): void {
    // Navigate to unit details page
    this.router.navigate(['/unitDetails', unitId]);
  }

  forgotPassowrd() {
    this.spinner.show();
    this.authService.forgotPassword({ email: this.email }).subscribe({
      next: (res) => {
        this.spinner.hide();
        Swal.fire({
          title: 'Success',
          text: res.msg,
          icon: 'success',
          draggable: true,
          confirmButtonText: 'Ok',
        });
      },
      error: (error) => {
        this.spinner.hide();
        console.log(error);
      },
    });
  }

  changePassowrd() {
    this.router.navigateByUrl('/change-password');
  }
}
