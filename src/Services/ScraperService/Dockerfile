FROM node:18

WORKDIR /app

COPY package*.json ./
COPY . .

# Restore service account key
RUN npm install
RUN npx playwright install-deps chromium
RUN npx playwright install chromium
RUN npm run build


EXPOSE 3000
CMD [ "npm", "start" ]