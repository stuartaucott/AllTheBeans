import { PublicClientApplication, LogLevel } from '@azure/msal-browser';
import type { Configuration } from '@azure/msal-browser';

const clientId = import.meta.env.VITE_AAD_CLIENT_ID;
export const apiScope = import.meta.env.VITE_API_SCOPE as string;

const config: Configuration = {
    auth: {
        clientId,
        authority: `https://login.microsoftonline.com/ed8cb39b-c5bb-4b8a-a184-3552ddb291ca`,
        redirectUri: 'http://localhost:5173/'
    },
    cache: { cacheLocation: 'sessionStorage' },
    system: {
        loggerOptions: { logLevel: LogLevel.Warning, piiLoggingEnabled: false }
    }
};

export const msalInstance = new PublicClientApplication(config);
export const loginRequest = { scopes: [apiScope] };