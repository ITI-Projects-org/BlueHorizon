
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/',
  locale: undefined,
  routes: [
  {
    "renderMode": 2,
    "redirectTo": "/home",
    "route": "/"
  },
  {
    "renderMode": 2,
    "route": "/home"
  },
  {
    "renderMode": 2,
    "route": "/login"
  },
  {
    "renderMode": 2,
    "route": "/register"
  },
  {
    "renderMode": 2,
    "route": "/VerifyOwner"
  },
  {
    "renderMode": 2,
    "route": "/PendingOwners"
  }
],
  entryPointToBrowserMapping: undefined,
  assets: {
    'index.csr.html': {size: 433, hash: '9143581cc0399bab4f87173235999e9df5ed0e4829557728bcf17002ecb1c686', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 946, hash: 'c56365105530b63a71350b41eab7cda0ab035b826d8067f764551febfd655b59', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'login/index.html': {size: 686, hash: 'a71e3d8f191a1520bcf6e95f50b29fb3fdab6a61f419df28d4d9d90136a4d62b', text: () => import('./assets-chunks/login_index_html.mjs').then(m => m.default)},
    'PendingOwners/index.html': {size: 718, hash: 'a2cc09f482a245181c8b71b27cd4448bdb05cad5f4786e18c531c8b77dacac15', text: () => import('./assets-chunks/PendingOwners_index_html.mjs').then(m => m.default)},
    'home/index.html': {size: 910, hash: 'eab5b8139cd6c7259c8e7d0b4ef93cfd316f1215ce2251e3cd929d066530af0f', text: () => import('./assets-chunks/home_index_html.mjs').then(m => m.default)},
    'register/index.html': {size: 2655, hash: '009410dd1714cee005b73d51ae2228b1af6c8f583ed686ec79f9e3329437dcd1', text: () => import('./assets-chunks/register_index_html.mjs').then(m => m.default)},
    'VerifyOwner/index.html': {size: 5937, hash: '50b3a407f3779ff73157f5ac253d438892b4a942ac3e47c8d1c73a43d08ede4d', text: () => import('./assets-chunks/VerifyOwner_index_html.mjs').then(m => m.default)},
    'styles-5INURTSO.css': {size: 0, hash: 'menYUTfbRu8', text: () => import('./assets-chunks/styles-5INURTSO_css.mjs').then(m => m.default)}
  },
};
