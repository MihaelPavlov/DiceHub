import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { TenantApplicationsService } from '../../../entities/common/api/tenant-applications.service';
import {
  ICompleteTenantSetupResult,
  ISeedGameCatalogDropdown,
} from '../../../entities/common/models/tenant-application.model';
import { IDropdown } from '../../../shared/models/dropdown.model';
import { TenantService } from '../../../shared/services/tenant.service';
import { FormDraftService } from '../../../shared/services/form-draft.service';

interface ITenantSetupForm {
  clubName: string;
  averageMaxCapacity: number | null;
  startWorkingHours: string;
  endWorkingHours: string;
  clubPhoneNumber: string;
  daysOff: string[];
  selectedGameIds: number[];
}

@Component({
  selector: 'app-tenant-setup',
  templateUrl: 'tenant-setup.component.html',
  styleUrl: 'tenant-setup.component.scss',
  standalone: false,
})
export class TenantSetupComponent implements OnInit, OnDestroy {
  private static readonly DraftKey = 'tenantSetup';
  private draftSubscription: Subscription | null = null;
  public form: FormGroup;
  public token: string | null = null;
  public isLoading = true;
  public isTokenValid = false;
  public isSaving = false;
  public isSubmitted = false;
  public setupResult: ICompleteTenantSetupResult | null = null;
  public seedGames: ISeedGameCatalogDropdown[] = [];
  public serverErrors: string[] = [];

  public readonly dayOptions: IDropdown[] = [
    { id: 1, name: 'Monday' },
    { id: 2, name: 'Tuesday' },
    { id: 3, name: 'Wednesday' },
    { id: 4, name: 'Thursday' },
    { id: 5, name: 'Friday' },
    { id: 6, name: 'Saturday' },
    { id: 7, name: 'Sunday' },
  ];

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly tenantService: TenantService,
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly formDraftService: FormDraftService
  ) {
    this.form = this.initFormGroup();
    this.draftSubscription = this.formDraftService.autoSave(this.form, TenantSetupComponent.DraftKey);
  }

  public ngOnDestroy(): void {
    this.draftSubscription?.unsubscribe();
  }

  public ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token');

    this.tenantService.validateTenantSetupToken(this.token).subscribe({
      next: (isValid) => {
        this.isTokenValid = isValid;

        if (!isValid) {
          this.isLoading = false;
          return;
        }

        this.loadSeedGames();
      },
      error: () => {
        this.isTokenValid = false;
        this.isLoading = false;
      },
    });
  }

  public submit(): void {
    this.serverErrors = [];

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const value = this.form.getRawValue() as ITenantSetupForm;

    this.tenantApplicationsService
      .completeSetup({
        token: this.token ?? '',
        clubName: value.clubName,
        averageMaxCapacity: value.averageMaxCapacity ?? 0,
        startWorkingHours: value.startWorkingHours,
        endWorkingHours: value.endWorkingHours,
        clubPhoneNumber: value.clubPhoneNumber,
        daysOff: value.daysOff,
        selectedGameIds: value.selectedGameIds,
      })
      .subscribe({
        next: (result) => {
          this.setupResult = result;
          this.isSubmitted = true;
          this.formDraftService.clear(TenantSetupComponent.DraftKey);
        },
        error: (error) => {
          this.serverErrors = this.extractErrors(error);
        },
        complete: () => {
          this.isSaving = false;
        },
      });
  }

  public navigateToTenantLogin(): void {
    if (!this.setupResult?.tenantId) return;

    this.router.navigateByUrl(`/${this.setupResult.tenantId}/login`);
  }

  public navigateToApplication(): void {
    this.router.navigateByUrl('/venue-application');
  }

  private loadSeedGames(): void {
    this.tenantApplicationsService.getSetupSeedGames().subscribe({
      next: (games) => {
        this.seedGames = games ?? [];
      },
      error: () => {
        this.serverErrors = ['Seed games could not be loaded.'];
      },
      complete: () => {
        this.isLoading = false;
      },
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      clubName: new FormControl<string>('', [
        Validators.required,
        Validators.maxLength(120),
      ]),
      averageMaxCapacity: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
      startWorkingHours: new FormControl<string>('', [Validators.required]),
      endWorkingHours: new FormControl<string>('', [Validators.required]),
      clubPhoneNumber: new FormControl<string>('', [
        Validators.required,
        Validators.maxLength(20),
        Validators.pattern(/^\+?[0-9\s-]{7,20}$/),
      ]),
      daysOff: new FormControl<string[]>([]),
      selectedGameIds: new FormControl<number[]>([], [Validators.required]),
    });
  }

  private extractErrors(error: any): string[] {
    const errors = error?.error?.errors;
    if (!errors) return ['Tenant setup failed.'];

    return Object.values(errors).flat() as string[];
  }
}
