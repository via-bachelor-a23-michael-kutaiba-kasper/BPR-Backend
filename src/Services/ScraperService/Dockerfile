FROM node:18

WORKDIR /app

ARG SERVICE_ACCOUNT_KEY=NOTHING

COPY package*.json ./
COPY . .

# Restore service account key
RUN npm install
RUN npx playwright install-deps
RUN npx playwright install
RUN npm run build

# ENV SERVICE_ACCOUNT_KEY_JSON=${SERVICE_ACCOUNT_KEY}


EXPOSE 3000
CMD [ "npm", "start" ]