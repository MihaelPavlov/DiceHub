import { NgModule } from '@angular/core';

import { provideFirestore, getFirestore } from '@angular/fire/firestore';
import { getMessaging, provideMessaging } from '@angular/fire/messaging';
import { provideFirebaseApp, initializeApp } from '@angular/fire/app';
import { getAuth, provideAuth } from '@angular/fire/auth';
import { environment } from '../app/environment';

@NgModule({
  imports: [
    provideFirebaseApp(() => initializeApp(environment.firebase)),
    provideFirestore(() => getFirestore()),
    provideAuth(() => getAuth()),
    provideMessaging(() => getMessaging()), // 👈👈👈 importing messaging module
  ],
})
export class FirebaseModule {}