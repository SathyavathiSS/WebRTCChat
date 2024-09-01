const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8080/chathub")  // URL of your SignalR Hub
    .build();

let localStream;
let localPeerConnection;
let remotePeerConnection;
const configuration = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };

document.getElementById("sendMessage").addEventListener("click", async () => {
    const message = document.getElementById("messageInput").value;
    await connection.invoke("SendMessage", "User", message);
});

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

connection.on("ReceiveOffer", async (offer) => {
    await localPeerConnection.setRemoteDescription(new RTCSessionDescription(offer));
    const answer = await localPeerConnection.createAnswer();
    await localPeerConnection.setLocalDescription(answer);
    connection.invoke("SendAnswer", "remoteConnectionId", answer);
});

connection.on("ReceiveAnswer", async (answer) => {
    await remotePeerConnection.setRemoteDescription(new RTCSessionDescription(answer));
});

connection.on("ReceiveIceCandidate", async (candidate) => {
    await remotePeerConnection.addIceCandidate(new RTCIceCandidate(candidate));
});

async function setupWebRTC() {
    localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
    document.getElementById("localVideo").srcObject = localStream;

    localPeerConnection = new RTCPeerConnection(configuration);
    remotePeerConnection = new RTCPeerConnection(configuration);

    localStream.getTracks().forEach(track => {
        localPeerConnection.addTrack(track, localStream);
    });

    localPeerConnection.onicecandidate = event => {
        if (event.candidate) {
            connection.invoke("SendIceCandidate", "remoteConnectionId", event.candidate);
        }
    };

    remotePeerConnection.onicecandidate = event => {
        if (event.candidate) {
            connection.invoke("SendIceCandidate", "localConnectionId", event.candidate);
        }
    };

    remotePeerConnection.ontrack = event => {
        document.getElementById("remoteVideo").srcObject = event.streams[0];
    };

    // Create an offer and set local description
    const offer = await localPeerConnection.createOffer();
    await localPeerConnection.setLocalDescription(offer);
    connection.invoke("SendOffer", "remoteConnectionId", offer);
}

connection.start().then(() => {
    setupWebRTC().catch(err => console.error("Error setting up WebRTC", err));
}).catch(err => console.error("Error connecting to SignalR hub", err));
