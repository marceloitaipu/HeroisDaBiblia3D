const CACHE_NAME = 'herois-biblia-v4.0.0';
const urlsToCache = [
  './',
  './index.html',
  './manifest.json',
  './TemplateData/style.css',
  './TemplateData/favicon.ico',
  './Build/docs.loader.js'
];

// Install Service Worker
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => {
        console.log('Cache aberto');
        return cache.addAll(urlsToCache);
      })
  );
  self.skipWaiting();
});

// Activate Service Worker
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames.map(cacheName => {
          if (cacheName !== CACHE_NAME) {
            console.log('Cache antigo removido:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
  self.clients.claim();
});

// Fetch - Network First Strategy (para jogos Unity)
self.addEventListener('fetch', event => {
  event.respondWith(
    fetch(event.request)
      .then(response => {
        // Clone response para cache
        const responseToCache = response.clone();
        caches.open(CACHE_NAME)
          .then(cache => {
            cache.put(event.request, responseToCache);
          });
        return response;
      })
      .catch(() => {
        // Se falhar, busca do cache
        return caches.match(event.request)
          .then(response => {
            if (response) {
              return response;
            }
            // Fallback para index.html
            return caches.match('./index.html');
          });
      })
  );
});
