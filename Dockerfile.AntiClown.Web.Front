FROM node:19.2-alpine as build

ENV NODE_ENV=production
COPY . .
RUN npm install --production
RUN npm run build
COPY . .

CMD ["npx", "serve", "-s", "build", "-l", "6820"]