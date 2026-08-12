export enum TenantApplicationStatus {
  PendingVerification = 0,
  Verified = 1,
  Rejected = 2,
}

export interface ITenantApplicationRequest {
  applicantType: string;
  contactName: string;
  email: string;
  phoneNumber: string;
  isEmailVerified: boolean;
  isPhoneVerified: boolean;
  address: string;
  publicWebsite: string;
  socialPage: string;
  discordServer: string;
  photoUrl: string;
}

export interface ITenantApplicationSendEmailCodeRequest {
  email: string;
  language?: string;
}

export interface ITenantApplicationVerifyEmailCodeRequest {
  email: string;
  code: string;
}

export interface ITenantApplication {
  id: number;
  applicantType: string;
  contactName: string;
  email: string;
  phoneNumber: string;
  isEmailVerified: boolean;
  isPhoneVerified: boolean;
  address: string;
  publicWebsite: string;
  socialPage: string;
  discordServer: string;
  photoUrl: string;
  status: TenantApplicationStatus;
  createdDate: string;
  reviewedDate?: string;
  reviewedByUserId?: string;
  reviewNote?: string;
}

export interface ITenantApplicationReviewRequest {
  note?: string;
}

export interface ICompleteTenantSetupRequest {
  token: string;
  clubName: string;
  averageMaxCapacity: number;
  startWorkingHours: string;
  endWorkingHours: string;
  clubPhoneNumber: string;
  daysOff: string[];
  selectedGameIds: number[];
}

export interface ICompleteTenantSetupResult {
  tenantId: string;
  tenantName: string;
  ownerEmail: string;
}

export interface ISeedGameCatalogDropdown {
  id: number;
  name: string;
  categoryName: string;
  minPlayers: number;
  maxPlayers: number;
  minAge: number;
  averagePlaytime: number;
  imageUrl: string;
  imageFileName: string;
}
