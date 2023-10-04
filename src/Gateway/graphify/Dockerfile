FROM node:18-alpine AS final
WORKDIR /app
COPY . .
RUN npm i
EXPOSE 8080
ENTRYPOINT [ "npm" ]
CMD [ "start" ]
