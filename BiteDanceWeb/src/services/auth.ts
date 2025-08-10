import { User, WebStorageStateStore } from "oidc-client-ts";

export const oidcConfig = {
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: import.meta.env.VITE_REDIRECT_URI,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  scope: import.meta.env.VITE_SCOPE,
};

export async function acquireToken(): Promise<string | null> {
  const oidcStorage = localStorage.getItem(
    `oidc.user:${oidcConfig.authority}:${oidcConfig.client_id}`
  );
  if (!oidcStorage) {
    return null;
  }

  return User.fromStorageString(oidcStorage).access_token;
}
