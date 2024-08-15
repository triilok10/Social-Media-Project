importScripts('/js/firebase-app.js');
importScripts('/js/firebase-messaging.js');

const firebaseConfig = {
    apiKey: "AIzaSyBpvaTISi5cJLmO71U4PsvDgLlGhFYlk70",
    authDomain: "socialmediaproject-60457.firebaseapp.com",
    projectId: "socialmediaproject-60457",
    storageBucket: "socialmediaproject-60457.appspot.com",
    messagingSenderId: "492049714309",
    appId: "1:492049714309:web:8c603d2416bff737f5adec"
};

firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

messaging.onBackgroundMessage(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body,
        icon: '/firebase-logo.png' // Customize the icon if needed
    };

    self.registration.showNotification(notificationTitle, notificationOptions);
});
