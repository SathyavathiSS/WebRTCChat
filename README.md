# WebRTCChat

# Solution Overview
AuthenticationService: For user registration, authentication
UserService: For managing user profiles
SharedData: Common files shared between AuthenticationService and UserService.
WebRTCService: For signaling, handling peer connections and managing chat messages and chat rooms.
Frontend: For the user interface.

# Project Structure
WebRTCChatApp/
├── AuthenticationService/
│   ├── Controllers/
│   │   └── AuthController.cs
│   ├── Migrations/
│   ├── Models/
│   │   ├── LoginModel.cs
│   │   ├── RegisterModel.cs
│   ├── Utilities/
│   │   └── PasswordHasher.cs
│   ├── Authentication.csproj
│   ├── Dockerfile
│   ├── Program.cs
│   ├── RemoteCertificateValidationCallback.cs
│   └── SetCertificateValidationCallback.cs
│   └── ... other files
│
├── Shared/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Models/
│   │   └── User.cs
│   ├── Utilities/
│   │   └── JwtTokenService.cs
│   ├── SharedData.csproj
│
├── UserService/
│   ├── Controllers/
│   │   └── UserProfileController.cs
│   ├── Migrations/
│   ├── Models/
│   │   ├── UpdateProfileDto.cs
│   │   └── UserProfileDto.cs
│   ├── ... other files
│   ├── UserService.csproj
│   ├── Dockerfile
│   └── Program.cs
│
├── WebRTCChatService/
│   ├── Controllers/
│   │   └── ChatController.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Models/
│   │   ├── ChatRoom.cs
│   │   └── Message.cs
│   ├── Services/
│   │   ├── ChatService.cs
│   │   └── IChatService.cs
│   ├── Hubs/
│   │   └── ChatHub.cs
│   ├── Program.cs
│   ├── Dockerfile
│   └── ... other files
│
├── FrontEnd/
│   ├── index.html
│   ├── styles.css
│   ├── script.js
│   ├── ... other files
│
├── docker-compose.yml
└── ... other files

# API List
Register: /api/auth/register
Login: /api/auth/login
validate Token: api/auth/test

Get user profile: /api/user/user-profile
Update user profile: /api/UserProfile

Create room: /api/chat/create-room
Get rooms: /api/chat/get-rooms
Send chat message: /chat/api/send-message
Connect to hub: /ws

# Documentation
AuthenticationService Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8080.app.github.dev/swagger
UserService Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8081.app.github.dev/swagger
WebRTCSErvice Documentation - https://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev/swagger

