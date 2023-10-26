FROM node:18

WORKDIR /app

COPY package*.json ./
COPY . .

# Restore service account key
RUN npm install
RUN npm run build

CMD [ "npm", "start" ]