import React, { useEffect } from 'react';
import SimpleWebRTC from 'simplewebrtc';

const ChatComponent = () => {
  useEffect(() => {
    const webrtc = new SimpleWebRTC({
      localVideoEl: 'localVideo',
      remoteVideosEl: 'remoteVideos',
      autoRequestMedia: true,
    });

    webrtc.on('readyToCall', () => {
      webrtc.joinRoom('chatroom');
    });
  }, []);

  return (
    <div>
      <video id="localVideo" />
      <div id="remoteVideos" />
    </div>
  );
};

export default ChatComponent;
