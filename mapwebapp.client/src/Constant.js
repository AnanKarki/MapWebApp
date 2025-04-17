// src/constants.js
export const AZURE_MAPS_CONFIG = {
    //CLIENT_ID: 'dc794a84-c66e-4012-8dee-b58850f11ae8',
    //SUBSCRIPTION_KEY: 'AdmoWQZGM90hA8ts2y6rAjMfqZCFSKGobK5rY7HQav4mICyMImNkJQQJ99BDACYeBjFMybzgAAAgAZMPRWHb'
    CLIENT_ID: import.meta.env.VITE_CLIENT_ID,
    SUBSCRIPTION_KEY: import.meta.env.VITE_SUBSCRIPTION_KEY
};

export const AZURE_MAPS_URLS = {
    MAP_SOURCE: 'https://atlas.microsoft.com/sdk/javascript/mapcontrol/3/atlas.min.js',
    LOCAL_DISTRICT_BOUNDARIES: import.meta.env.VITE_API_BASE_URL + 'azuremaps/uk-boundaries',
    DISTRICT_LIST: import.meta.env.VITE_API_BASE_URL +'azuremaps/getdistricts',
    POLYGON_BY_REFERENCE: import.meta.env.VITE_API_BASE_URL + 'azuremaps/getpolygonbyreference?reference={reference}'
};
