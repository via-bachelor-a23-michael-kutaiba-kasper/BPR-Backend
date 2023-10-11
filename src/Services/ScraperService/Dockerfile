FROM node:18

WORKDIR /app

ARG SERVICE_ACCOUNT_KEY=NOTHING

COPY package*.json ./
COPY . .

# Restore service account key
RUN echo ${SERVICE_ACCOUNT_KEY} > /app/service_account_key.json

RUN npm install
RUN npx playwright install-deps
RUN npx playwright install
RUN npm run build

ENV GOOGLE_APPLICATION_CREDENTIALS=/app/service_account_key.json


EXPOSE 3000
CMD [ "npm", "start" ]