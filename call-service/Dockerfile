FROM node:latest

WORKDIR /app

COPY package*.json ./

RUN npm config set strict-ssl false

RUN npm install

COPY . .

EXPOSE 8080

CMD ["npx", "webpack", "serve", "--config", "webpack.config.js"]