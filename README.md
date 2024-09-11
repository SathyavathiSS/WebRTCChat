# WebRTCChat

## Solution Overview
AuthenticationService: For user registration, authentication
UserService: For managing user profiles
SharedData: Common files shared between AuthenticationService and UserService.
WebRTCService: For signaling, handling peer connections and managing chat messages and chat rooms.
Frontend: For the user interface.

## Project Structure
![image](https://github.com/user-attachments/assets/90873ef4-2187-441a-909f-e4a446557525)

## API List
Register: /api/auth/register

Login: /api/auth/login

validate Token: api/auth/test

Get user profile: /api/user/user-profile

Update user profile: /api/UserProfile


Create room: /api/chat/create-room

Get rooms: /api/chat/get-rooms

Send chat message: /chat/api/send-message

Connect to hub: /ws

## API Documentation
AuthenticationService Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8080.app.github.dev/swagger

UserService Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8081.app.github.dev/swagger 

WebRTCSErvice Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev/swagger

