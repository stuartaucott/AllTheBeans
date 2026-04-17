import { PublicClientApplication, LogLevel } from '@azure/msal-browser';
import type { Configuration } from '@azure/msal-browser';

const tenantId = import.meta.env.VITE_AAD_TENANT_ID;
const clientId = import.meta.env.VITE_AAD_CLIENT_ID;
export const apiScope = import.meta.env.VITE_API_SCOPE as string;

const config: Configuration = {
    auth: {
        clientId,
        authority: `https://login.microsoftonline.com/${tenantId}`,
        redirectUri: window.location.origin
    },
    cache: { cacheLocation: 'sessionStorage' },
    system: {
        loggerOptions: { logLevel: LogLevel.Warning, piiLoggingEnabled: false }
    }
};

export const msalInstance = new PublicClientApplication(config);
export const loginRequest = { scopes: [apiScope] };