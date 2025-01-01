//const APP_ID = "8b1a88e0327747e399829f7cac96c38d"
//const TOKEN = "007eJxTYFhi1cXNLslbPGU/X96zc9kHPh75saW7a4PvVdV/z1abygUrMFgkGSZaWKQaGBuZm5uYpxpbWloYWaaZJycmW5olG1ukrLyZlt4QyMigk5jEysgAgSA+C0NuYmYeAwMAZMQfuQ=="
//const CHANNEL = "main"

//const client = AgoraRTC.createClient({ mode: 'rtc', codec: 'vp8' })
////AgoraRTC.setParameter("rtc.udpProxyFallback", true);
////AgoraRTC.setParameter("rtc.statsReportInterval", 500);
////client.enableDualStream();
////client.setLowStreamParameter({
////    width: 320,
////    height: 180,
////    framerate: 15,
////    bitrate: 200
////});

//let localTracks = []
//let remoteUsers = {}

//let joinAndDisplayLocalStream = async () => {

//    client.on('user-published', handleUserJoined)
    
//    client.on('user-left', handleUserLeft)
    
//    let UID = await client.join(APP_ID, CHANNEL, TOKEN, null)

//    localTracks = await AgoraRTC.createMicrophoneAndCameraTracks({}, {
//        encoderConfig: {
//            width: { ideal: 160, max: 160 },
//            height: { ideal: 120, max: 120 },
//            bitrateMin: 10,  // Min bitrate in Kbps
//            bitrateMax: 1500 // Max bitrate in Kbps
//        }
//    });

//    let player = `<div class="video-container" id="user-container-${UID}">
//                        <div class="video-player" id="user-${UID}"></div>
//                  </div>`
//    document.getElementById('video-streams').insertAdjacentHTML('beforeend', player)

//    localTracks[1].play(`user-${UID}`)
    
//    await client.publish([localTracks[0], localTracks[1]])
//}

//window.joinStream = async () => {
//    await joinAndDisplayLocalStream()
//    document.getElementById('join-btn').style.display = 'none'
//    document.getElementById('stream-controls').style.display = 'flex'
//}

//let handleUserJoined = async (user, mediaType) => {
//    remoteUsers[user.uid] = user 
//    await client.subscribe(user, mediaType)

//    if (mediaType === 'video'){
//        let player = document.getElementById(`user-container-${user.uid}`)
//        if (player != null){
//            player.remove()
//        }

//        player = `<div class="video-container" id="user-container-${user.uid}">
//                        <div class="video-player" id="user-${user.uid}"></div> 
//                 </div>`
//        document.getElementById('video-streams').insertAdjacentHTML('beforeend', player)

//        user.videoTrack.play(`user-${user.uid}`)
//    }

//    if (mediaType === 'audio'){
//        user.audioTrack.play()
//    }
//}

//let handleUserLeft = async (user) => {
//    delete remoteUsers[user.uid]
//    document.getElementById(`user-container-${user.uid}`).remove()
//}

//window.leaveAndRemoveLocalStream = async () => {
//    for(let i = 0; localTracks.length > i; i++){
//        localTracks[i].stop()
//        localTracks[i].close()
//    }

//    await client.leave()
//    document.getElementById('join-btn').style.display = 'block'
//    document.getElementById('stream-controls').style.display = 'none'
//    document.getElementById('video-streams').innerHTML = ''
//}

//window.toggleMic = async () => {
//    let cameraBtn = document.getElementById('mic-btn');
//    if (localTracks[0].muted){
//        await localTracks[0].setMuted(false)
//        cameraBtn.innerText = 'Mic on'
//        cameraBtn.style.backgroundColor = 'cadetblue'
//    }else{
//        await localTracks[0].setMuted(true)
//        cameraBtn.innerText = 'Mic off'
//        cameraBtn.style.backgroundColor = '#EE4B2B'
//    }
//}

//window.toggleCamera = async () => {
//    const videoTrack = localTracks[1];
//    let cameraBtn = document.getElementById('camera-btn');
//    cameraBtn.innerText = videoTrack.enabled ? 'Camera Off' : 'Camera On';
//    cameraBtn.style.backgroundColor = videoTrack.enabled ? '#EE4B2B' : 'cadetblue';

//    if (videoTrack.enabled) {
//        await videoTrack.setEnabled(false); // Disable video
//        //e.target.innerText = 'Camera Off';
//        //e.target.style.backgroundColor = '#EE4B2B';
//    } else {
//        await videoTrack.setEnabled(true); // Enable video
//        //e.target.innerText = 'Camera On';
//        //e.target.style.backgroundColor = 'cadetblue';
//    }
//};


////document.getElementById('join-btn').addEventListener('click', joinStream)
////document.getElementById('leave-btn').addEventListener('click', leaveAndRemoveLocalStream)
////document.getElementById('mic-btn').addEventListener('click', toggleMic)
////document.getElementById('camera-btn').addEventListener('click', toggleCamera)