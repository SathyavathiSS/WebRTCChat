// script.js
let localStream;
let pc;
let signalingChannel;
let remotePeerConnection;

function joinRoom() {
    const username = document.getElementById('username').value;
    const roomName = prompt("Enter room name");
    if (!roomName) return;

    signalingChannel = io.connect('http://localhost:3000');
    signalingChannel.emit('join', roomName);

    signalingChannel.on('offer', handleOffer);
    signalingChannel.on('answer', handleAnswer);
    signalingChannel.on('candidate', handleCandidate);

    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
            localStream = stream;
            document.getElementById('localVideo').srcObject = stream;
        })
        .catch(err => console.error('Error accessing camera/microphone:', err));
}

function handleOffer(data) {
    pc = new RTCPeerConnection();
    remotePeerConnection = pc;

    pc.addTrack(localStream.getTracks()[0], localStream);
    pc.addTrack(localStream.getTracks()[1], localStream);

    pc.createDataChannel('chat');

    pc.ontrack = event => {
        document.getElementById('remoteVideo').srcObject = event.streams[0];
    };

    pc.onicecandidate = event => {
        if (event.candidate) {
            signalingChannel.emit('candidate', event.candidate, 'room-name');
        }
    };

    pc.setRemoteDescription(new RTCSessionDescription({ type: 'offer', sdp: data }));
}

function handleAnswer(data) {
    pc.setRemoteDescription(new RTCSessionDescription({ type: 'answer', sdp: data }));
}

function handleCandidate(event) {
    pc.addIceCandidate(new RTCIceCandidate(event.candidate));
}