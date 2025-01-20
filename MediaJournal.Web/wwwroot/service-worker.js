const staticCacheName = "media-journal-static-cache-4";
const runtimeCacheName = "media-journal-runtime-cache-4";
const bootstrapIconsCdnCss = "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css";

const CACHE_DURATION = 24 * 60 * 60 * 1000;
const AUTH_REFRESH_INTERVAL = 60 * 60 * 1000;

const staticCacheUrls = [
    '/',
    '/Identity/Account/Login',
    '/Identity/Account/Register',
    '/css/site.css',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    bootstrapIconsCdnCss,
    '/js/site.js',
    '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
    '/icons/icon-192x192.png',
    '/icons/icon-512x512.png',
    '/manifest.json',
    '/service-worker.js',
    '/favicon.ico',
    '/offline.html'
];

const createCacheableResponse = (response) => {
    const clonedResponse = response.clone();
    const newHeaders = new Headers(clonedResponse.headers);
    newHeaders.set('sw-cache-timestamp', new Date().toISOString());

    return new Response(clonedResponse.body, {
        status: clonedResponse.status,
        statusText: clonedResponse.statusText,
        headers: newHeaders
    });
};

const isCacheExpired = (cachedResponse, duration = CACHE_DURATION) => {
    if (!cachedResponse) return true;
    const cachedTime = new Date(cachedResponse.headers.get('sw-cache-timestamp'));
    return Date.now() - cachedTime.getTime() > duration;
};

const getMediaIdsFromDashboard = async (dashboardHtml) => {
    const mediaDataMatch = dashboardHtml.match(/data-media-ids='(\[.*?\])'/);
    if (!mediaDataMatch || !mediaDataMatch[1]) {
        return [];
    }

    try {
        return JSON.parse(mediaDataMatch[1]);
    } catch (error) {
        console.error('Failed to parse media IDs:', error);
        return [];
    }
};

const refreshAuthenticatedCache = async (cache) => {
    const dashboardResponse = await fetch('/Media/Dashboard', {
        credentials: 'same-origin',
        cache: 'no-cache'
    });

    if (!dashboardResponse.ok) {
        throw new Error('Failed to fetch dashboard');
    }

    await cache.put(
        '/Media/Dashboard',
        createCacheableResponse(dashboardResponse.clone())
    );

    const dashboardHtml = await dashboardResponse.text();
    const mediaIds = await getMediaIdsFromDashboard(dashboardHtml);

    const indexResponse = await fetch('/Media/Index', {
        credentials: 'same-origin',
        cache: 'no-cache'
    });
    if (indexResponse.ok) {
        await cache.put(
            '/Media/Index',
            createCacheableResponse(indexResponse)
        );
    }

    await Promise.all((await cache.keys())
        .filter(req => req.url.includes('/Media/Details/'))
        .map(req => cache.delete(req)));

    await Promise.all(mediaIds.map(async (id) => {
        const detailsResponse = await fetch(`/Media/Details/${id}`, {
            credentials: 'same-origin',
            cache: 'no-cache'
        });
        if (detailsResponse.ok) {
            await cache.put(
                `/Media/Details/${id}`,
                createCacheableResponse(detailsResponse)
            );
        }
    }));

    const timestamp = new Date().getTime();
    await cache.put(
        '/__auth_refresh_timestamp',
        new Response(timestamp.toString())
    );
};

const shouldRefreshAuth = async (cache) => {
    const timestampResponse = await cache.match('/__auth_refresh_timestamp');
    if (!timestampResponse) return true;

    const lastRefresh = parseInt(await timestampResponse.text());
    return Date.now() - lastRefresh > AUTH_REFRESH_INTERVAL;
};

const getPublicIdsFromHtml = async (html) => {
    const mediaDataMatch = html.match(/data-media-ids='(\[.*?\])'/);
    if (!mediaDataMatch || !mediaDataMatch[1]) {
        return [];
    }

    try {
        return JSON.parse(mediaDataMatch[1]);
    } catch (error) {
        console.error('Failed to parse public media IDs:', error);
        return [];
    }
};

const shouldRefreshPublicCache = async (cache) => {
    const hasPublicEntries = (await cache.keys())
        .some(req => req.url.includes('/Public/'));

    if (!hasPublicEntries) return true;

    const indexResponse = await cache.match('/Public/Index');
    if (!indexResponse) return true;

    const cachedTime = new Date(indexResponse.headers.get('sw-cache-timestamp'));
    return Date.now() - cachedTime.getTime() > 10 * 60 * 1000;
};

const refreshPublicCache = async (cache) => {
    const publicResponse = await fetch('/Public/Index', {
        cache: 'no-cache'
    });

    if (!publicResponse.ok) {
        throw new Error('Failed to fetch public reviews');
    }

    await cache.put(
        '/Public/Index',
        createCacheableResponse(publicResponse.clone())
    );

    const publicHtml = await publicResponse.text();
    const publicIds = await getPublicIdsFromHtml(publicHtml);

    await Promise.all((await cache.keys())
        .filter(req => req.url.includes('/Public/Details/'))
        .map(req => cache.delete(req)));

    await Promise.all(publicIds.map(async (id) => {
        const detailsResponse = await fetch(`/Public/Details/${id}`, {
            cache: 'no-cache'
        });
        if (detailsResponse.ok) {
            await cache.put(
                `/Public/Details/${id}`,
                createCacheableResponse(detailsResponse)
            );
        }
    }));
};

const ensureOfflinePage = async (cache) => {
    const existingOfflinePage = await cache.match('/offline.html');
    if (!existingOfflinePage) {
        try {
            const offlineResponse = await fetch('/offline.html');
            if (offlineResponse.ok) {
                await cache.put('/offline.html', createCacheableResponse(offlineResponse));
            }
        } catch (error) {
            console.error('Failed to cache offline page:', error);
        }
    }
};

self.addEventListener('install', (event) => {
    event.waitUntil(
        (async () => {
            const cache = await caches.open(staticCacheName);

            await Promise.all(
                staticCacheUrls.map(async (url) => {
                    try {
                        const response = await fetch(url);
                        if (response.ok) {
                            await cache.put(url, createCacheableResponse(response));
                        }
                    } catch (error) {
                        console.error(`Failed to cache ${url}:`, error);
                    }
                })
            );

            await self.skipWaiting();
        })()
    );
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        (async () => {
            const cacheNames = await caches.keys();
            const cachesToDelete = cacheNames.filter(
                name => name !== staticCacheName && name !== runtimeCacheName
            );
            await Promise.all(cachesToDelete.map(name => caches.delete(name)));
            await self.clients.claim();
        })()
    );
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        (async () => {
            const cache = await caches.open(runtimeCacheName);

            await ensureOfflinePage(cache);
            
            const isNotGet = ['POST', 'PUT', 'DELETE'].includes(event.request.method);
            const mediaChanged = isNotGet && event.request.url.includes('/Media/');

            const needsPublicRefresh = await shouldRefreshPublicCache(cache);
            const isPublicEndpoint = event.request.url.includes('/Public/');

            if (isPublicEndpoint || needsPublicRefresh) {
                try {
                    await refreshPublicCache(cache);
                } catch (refreshError) {
                    console.error('Failed to refresh public cache:', refreshError);
                }
            }
            
            try {
                if (isNotGet) {
                    const response = await fetch(event.request);

                    if (mediaChanged && response.ok) {
                        try {
                            await refreshAuthenticatedCache(cache);
                        } catch (refreshError) {
                            console.error('Failed to refresh cache after mutation:', refreshError);
                        }
                    }

                    return response;
                }

                const isAuthEndpoint = event.request.url.includes('/Media/');

                if (isAuthEndpoint) {
                    try {
                        await refreshAuthenticatedCache(cache);
                    } catch (refreshError) {
                        console.error('Failed to refresh authenticated cache:', refreshError);
                    }
                }

                const networkResponse = await fetch(event.request, {
                    cache: 'no-cache',
                    credentials: 'same-origin'
                });

                if (networkResponse.ok) {
                    const cacheableResponse = createCacheableResponse(networkResponse);
                    await cache.put(event.request, cacheableResponse.clone());
                    return cacheableResponse;
                }

                const cachedResponse = await cache.match(event.request);
                if (cachedResponse && !isCacheExpired(cachedResponse)) {
                    return cachedResponse;
                }

                return networkResponse;
            } catch (error) {
                const cachedResponse = await cache.match(event.request);
                if (cachedResponse) {
                    return cachedResponse;
                }

                if (event.request.mode === 'navigate') {
                    const offlinePage = await cache.match('/offline.html');
                    if (offlinePage) {
                        return offlinePage;
                    }
                }
                
                throw error;
            }
        })()
    );
});