FROM nginx:alpine

# Copy static assets
COPY frontend/index.html /usr/share/nginx/html/index.html
COPY frontend/styles.css /usr/share/nginx/html/styles.css
COPY frontend/assets/images/avatar.png /usr/share/nginx/html/assets/images/avatar.png
COPY frontend/script.js /usr/share/nginx/html/script.js

# Copy custom Nginx configuration
COPY ./nginx/templates/default.conf /etc/nginx/conf.d/default.conf

# Set environment variables
#ENV API_URL http://webrtc:8080/api
ENV API_HOST=auth
ENV USER_HOST=user
ENV WEBCRTC_HOST=webrtc

# Expose the port
EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]