// This file is overwritten by the Dockerfile at build time using the API_URL build arg.
// Do not edit manually — set API_URL in Render's environment variables instead.
export const environment = {
  production: true,
  apiUrl: 'https://your-api.onrender.com',
};
