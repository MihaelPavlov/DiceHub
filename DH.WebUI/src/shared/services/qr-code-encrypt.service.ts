import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({ providedIn: 'root' })
export class QrEncryptService {
  private secret = '0123456789ABCDEF0123456789ABCDEF'; // 32 chars

  // Encrypt synchronously with explicit IV
  encryptObjectSync(obj: any): string {
    const json = JSON.stringify(obj);
    const key = CryptoJS.enc.Utf8.parse(this.secret);

    // generate 16-byte IV
    const iv = CryptoJS.lib.WordArray.random(16);

    // Encrypt using AES-CBC, PKCS7 padding
    const encrypted = CryptoJS.AES.encrypt(json, key, {
      iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7,
    });

    // Prepend IV to ciphertext and convert to Base64
    const ivCipher = iv.concat(encrypted.ciphertext);
    return CryptoJS.enc.Base64.stringify(ivCipher);
  }

  // Decrypt synchronously (optional local validation)
  decryptObjectSync(encryptedBase64: string): string {
    const key = CryptoJS.enc.Utf8.parse(this.secret);

    // Convert from Base64 to WordArray
    const fullCipher = CryptoJS.enc.Base64.parse(encryptedBase64);

    // Extract IV (first 16 bytes) and ciphertext
    const iv = CryptoJS.lib.WordArray.create(fullCipher.words.slice(0, 4)); // 4 words = 16 bytes
    const ciphertext = CryptoJS.lib.WordArray.create(
      fullCipher.words.slice(4),
      fullCipher.sigBytes - 16
    );

    // Decrypt
    const decrypted = CryptoJS.AES.decrypt({ ciphertext }, key, {
      iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7,
    });
    return decrypted.toString(CryptoJS.enc.Utf8);
  }
}
