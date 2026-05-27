# Build Angular app
FROM node:22-alpine AS build
WORKDIR /app

COPY bHub/package*.json ./
RUN npm ci

COPY bHub/ ./
RUN npm run build

# Serve with nginx
FROM nginx:1.27-alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY bHub/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/bhub/browser/ /usr/share/nginx/html/

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
