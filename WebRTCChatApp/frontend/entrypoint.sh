#!/bin/sh
set -e

# Check environment variables
echo "API_HOST: $API_HOST"
echo "USER_HOST: $USER_HOST"
echo "WEBCRTC_HOST: $WEBCRTC_HOST"

# Start Nginx
exec nginx -g "daemon off;"
