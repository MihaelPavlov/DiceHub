importScripts(
  "https://www.gstatic.com/firebasejs/9.8.0/firebase-app-compat.js"
);
importScripts(
  "https://www.gstatic.com/firebasejs/9.8.0/firebase-messaging-compat.js"
);
const app = firebase.initializeApp({
  apiKey: "AIzaSyBSZ3ju-sFNOPAoLuw_q2PebmtQsQ3D13s",
  authDomain: "dicehub-8c63f.firebaseapp.com",
  projectId: "dicehub-8c63f",
  storageBucket: "dicehub-8c63f.appspot.com",
  messagingSenderId: "566204429306",
  appId: "1:566204429306:web:cda989326f8fd750d02a43",
  measurementId: "G-1SNNNLCVLW",
});

const messaging = firebase.messaging(app);

self.addEventListener('push', function (event) {
  if (!event.data) return;

  const data = event.data.json();

  // ❌ Ignore system update notifications
  if (data.notification?.title === 'dicehub.online') {
    console.log('Ignored system update notification');
    return;
  }

  // ✅ Extract from data payload
  const title = data.data?.title || data.title || 'Notification';
  const options = {
    body: data.data?.body || data.body,
    icon: data.data?.icon || data.icon,
    data: {
      click_action: data.data?.click_action || data.click_action || '/'
    }
  };

  event.waitUntil(
    self.registration.showNotification(title, options)
  );
});

// ✅ Handle clicks
self.addEventListener('notificationclick', function (event) {
  event.notification.close();
  const click_action = event.notification.data?.click_action || '/';

  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
      for (const client of clientList) {
        if (client.url === click_action && 'focus' in client) {
          return client.focus();
        }
      }
      return clients.openWindow(click_action);
    })
  );
});