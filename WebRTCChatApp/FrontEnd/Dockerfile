FROM nginx:alpine
COPY ./index.html /usr/share/nginx/html/index.html
COPY ./styles.css /usr/share/nginx/html/styles.css
COPY ./assets/images/avatar.png /usr/share/nginx/html/assets/images/avatar.png
COPY ./script.js /usr/share/nginx/html/script.js
ENV API_URL http://webrtc:8080/api
EXPOSE 8083
CMD ["nginx", "-g", "daemon off;"]
