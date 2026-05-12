# ── Build ─────────────────────────────────────────────────────────────────
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci --silent

COPY . .

# Inject the backend URL at build time (set API_URL in Render's build env vars)
ARG API_URL=http://localhost:5000
RUN printf "export const environment = { production: true, apiUrl: '%s' };\n" "$API_URL" \
    > src/environments/environment.prod.ts

RUN npm run build -- --configuration production

# ── Serve ─────────────────────────────────────────────────────────────────
FROM nginx:1.25-alpine AS runtime

# Copy built Angular app
COPY --from=build /app/dist/math-ui/browser /usr/share/nginx/html

# nginx 1.19+ auto-runs envsubst on files in /etc/nginx/templates/
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

# Render sets PORT automatically; default to 10000 (Render's default)
ENV PORT=10000
EXPOSE 10000
