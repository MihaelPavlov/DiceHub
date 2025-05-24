importScripts(
  'https://www.gstatic.com/firebasejs/9.8.0/firebase-app-compat.js',
);
importScripts(
  'https://www.gstatic.com/firebasejs/9.8.0/firebase-messaging-compat.js',
);
  const app = firebase.initializeApp({
    apiKey: "AIzaSyBSZ3ju-sFNOPAoLuw_q2PebmtQsQ3D13s",
    authDomain: "dicehub-8c63f.firebaseapp.com",
    projectId: "dicehub-8c63f",
    storageBucket: "dicehub-8c63f.appspot.com",
    messagingSenderId: "566204429306",
    appId: "1:566204429306:web:cda989326f8fd750d02a43",
    measurementId: "G-1SNNNLCVLW"
  });
  
  const messaging = firebase.messaging(app);
