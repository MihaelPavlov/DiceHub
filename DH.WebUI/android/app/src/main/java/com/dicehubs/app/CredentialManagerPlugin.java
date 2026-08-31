package com.dicehubs.app;

import android.os.CancellationSignal;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.credentials.CreateCredentialResponse;
import androidx.credentials.CreatePasswordRequest;
import androidx.credentials.Credential;
import androidx.credentials.CredentialManager;
import androidx.credentials.CredentialManagerCallback;
import androidx.credentials.GetCredentialRequest;
import androidx.credentials.GetCredentialResponse;
import androidx.credentials.GetPasswordOption;
import androidx.credentials.PasswordCredential;
import androidx.credentials.exceptions.CreateCredentialException;
import androidx.credentials.exceptions.GetCredentialException;
import androidx.credentials.exceptions.NoCredentialException;

import com.getcapacitor.JSObject;
import com.getcapacitor.Plugin;
import com.getcapacitor.PluginCall;
import com.getcapacitor.PluginMethod;
import com.getcapacitor.annotation.CapacitorPlugin;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/**
 * Bridges the AndroidX Credential Manager (Jetpack) to the web login screen.
 *
 * The web login's {@code navigator.credentials} path is a no-op inside the
 * Capacitor/Android System WebView (the Credential Management API's
 * {@code PasswordCredential} is not implemented there), so the native app has
 * to talk to Google Password Manager / other credential providers directly.
 *
 * Everything here is best-effort: a user declining the sheet, having no
 * provider, or no saved credential must never surface as an error to the login
 * flow. {@code savePassword} always resolves; {@code getPassword} resolves with
 * nulls when there is nothing to offer.
 */
@CapacitorPlugin(name = "CredentialManager")
public class CredentialManagerPlugin extends Plugin {

    private static final String TAG = "CredentialManager";

    private final Executor executor = Executors.newSingleThreadExecutor();
    private CredentialManager credentialManager;

    @Override
    public void load() {
        credentialManager = CredentialManager.create(getContext());
    }

    @PluginMethod
    public void isAvailable(PluginCall call) {
        JSObject ret = new JSObject();
        ret.put("available", credentialManager != null);
        call.resolve(ret);
    }

    @PluginMethod
    public void savePassword(PluginCall call) {
        String username = call.getString("username");
        String password = call.getString("password");
        if (username == null || password == null || username.isEmpty() || password.isEmpty()) {
            call.reject("username and password are required");
            return;
        }

        CreatePasswordRequest request = new CreatePasswordRequest(username, password);

        credentialManager.createCredentialAsync(
            getActivity(),
            request,
            new CancellationSignal(),
            executor,
            new CredentialManagerCallback<CreateCredentialResponse, CreateCredentialException>() {
                @Override
                public void onResult(CreateCredentialResponse result) {
                    call.resolve();
                }

                @Override
                public void onError(@NonNull CreateCredentialException e) {
                    // No provider, user dismissed the "save password?" sheet,
                    // "never for this app", etc. - none of these are failures
                    // as far as logging in is concerned.
                    Log.w(TAG, "savePassword: " + e.getClass().getSimpleName() + " - " + e.getMessage());
                    call.resolve();
                }
            }
        );
    }

    @PluginMethod
    public void getPassword(PluginCall call) {
        GetCredentialRequest request = new GetCredentialRequest.Builder()
            .addCredentialOption(new GetPasswordOption())
            .build();

        credentialManager.getCredentialAsync(
            getActivity(),
            request,
            new CancellationSignal(),
            executor,
            new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>() {
                @Override
                public void onResult(GetCredentialResponse result) {
                    Credential credential = result.getCredential();
                    if (credential instanceof PasswordCredential) {
                        PasswordCredential pc = (PasswordCredential) credential;
                        call.resolve(credentialResult(pc.getId(), pc.getPassword()));
                    } else {
                        call.resolve(credentialResult(null, null));
                    }
                }

                @Override
                public void onError(@NonNull GetCredentialException e) {
                    // NoCredentialException  -> nothing saved for this app.
                    // Cancellation/interrupted -> user backed out of the sheet.
                    // All map to "no credential to pre-fill".
                    if (!(e instanceof NoCredentialException)) {
                        Log.w(TAG, "getPassword: " + e.getClass().getSimpleName() + " - " + e.getMessage());
                    }
                    call.resolve(credentialResult(null, null));
                }
            }
        );
    }

    private static JSObject credentialResult(String username, String password) {
        JSObject ret = new JSObject();
        ret.put("username", username);
        ret.put("password", password);
        return ret;
    }
}
