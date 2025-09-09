FROM node:20.19 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .

# Use Angular production configuration (ensure package.json build uses prod)
RUN npm run build --if-present -- --configuration production

# Nginx stage
FROM nginx:1.25-alpine AS runtime

# copy custom nginx conf to enable SPA fallback and better caching
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built app (adjust output path if your build outputs different folder)
COPY --from=build /app/dist/FootballClient/browser /usr/share/nginx/html


EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]