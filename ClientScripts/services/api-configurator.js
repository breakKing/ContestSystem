import axios from "axios";

export function createApi() {
    return axios.create({
        baseURL: "/api",
        headers: {
          "Content-Type": "application/json",
        },
    })
}

export function configureApi(api, store) {
    api.interceptors.request.use(
        (config) => {
            if (store.getters.accessToken) {
                config.headers["Authorization"] = 'Bearer ' + store.getters.accessToken;
            }
            return config;
        },
        (error) => {
            return Promise.reject(error);
        }
    );
    
    api.interceptors.response.use(
        (res) => {
            return res;
        },
        async (error) => {
            const originalConfig = error.config;
            
            if (originalConfig && originalConfig.url && originalConfig.url !== "/auth/session/login" && error.response) {
                if (error.response.status === 401 && !originalConfig._retry) {
                    originalConfig._retry = true;

                    await store.dispatch("refreshToken");
                    
                    if(store.getters.accessToken) {
                        return api(originalConfig);
                    }
                    else {
                        return Promise.reject(error);
                    }
                }
            }
    
            return Promise.reject(error);
        }
    )

    store.commit('setApi', api);

    return api;
}