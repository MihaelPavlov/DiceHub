import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ROUTE } from '../../../shared/configs/route.config';
import { Form } from '../../../shared/components/form/form.component';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { PartnerInquiriesService } from '../../../entities/common/api/partner-inquiries.service';
import { ToastType } from '../../../shared/models/toast.model';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { NavigationService } from '../../../shared/services/navigation-service';
import { TranslateService } from '@ngx-translate/core';
import { FormDraftService } from '../../../shared/services/form-draft.service';

interface IPartnerInquiryForm {
  name: string;
  email: string;
  message: string;
  phoneNumber: string;
}

@Component({
  selector: 'app-landing',
  templateUrl: 'landing.component.html',
  styleUrl: 'landing.component.scss',
  standalone: false,
})
export class LandingComponent extends Form implements OnDestroy {
  override form: Formify<IPartnerInquiryForm>;

  /** Swaps the "story" thumbnail for the embedded YouTube player on click. */
  public storyPlaying = false;

  private static readonly DraftKey = 'landingPartnerInquiry';
  private draftSubscription: Subscription | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly partnerInquiriesService: PartnerInquiriesService,
    public override readonly toastService: ToastService,
    private readonly navigationService: NavigationService,
    public override translateService: TranslateService,
    private readonly formDraftService: FormDraftService
  ) {
    super(toastService, translateService);

    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.draftSubscription = this.formDraftService.autoSave(this.form, LandingComponent.DraftKey);
  }

  public ngOnDestroy(): void {
    this.draftSubscription?.unsubscribe();
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }
  public onLogin(): void {
    this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
  }

  public onRegister(): void {
    this.router.navigateByUrl(ROUTE.REGISTER);
  }

  public playStory(): void {
    this.storyPlaying = true;
  }

  public onInstructions(): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(ROUTE.INSTRUCTIONS);
  }
  public onSubmit(): void {
    if (this.form.valid) {
      this.partnerInquiriesService
        .create({
          name: this.form.controls.name.value,
          email: this.form.controls.email.value,
          phoneNumber: this.form.controls.phoneNumber.value,
          message: this.form.controls.message.value,
        })
        .subscribe({
          next: (response) => {
            this.toastService.success({
              message: 'Inquiry submitted successfully!',
              type: ToastType.Success,
            });
            // emitEvent: false - a normal reset() fires valueChanges, which would
            // schedule a new (blank) debounced autosave ~500ms later that clear()
            // below can't prevent, silently reviving an empty draft.
            this.form.reset({}, { emitEvent: false });
            this.formDraftService.clear(LandingComponent.DraftKey);
          },
          error: (error) => {
            this.handleServerErrors(error);
            this.toastService.error({
              message: AppToastMessage.FailedToSaveChanges,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'name':
        return 'Name';
      case 'email':
        return 'Email';
      case 'message':
        return 'Message';
      case 'phoneNumber':
        return 'Phone Number';
      default:
        return controlName;
    }
  }
  private initFormGroup(): FormGroup {
    return this.fb.group({
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      email: new FormControl<string>('', [Validators.required]),
      phoneNumber: new FormControl<string>('', [Validators.required]),
      message: new FormControl<string>('', [Validators.required]),
    });
  }
}
