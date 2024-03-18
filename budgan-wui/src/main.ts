import './assets/main.css'
import '@mdi/font/css/materialdesignicons.css'
import { createApp } from 'vue'
import { createVuetify } from 'vuetify';
import { createPinia } from 'pinia'
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import 'vuetify/styles';
import { container } from './setupInversify';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate';

import App from './App.vue'
import router from './router'


const app = createApp(App)
const pinia = createPinia();

pinia.use(piniaPluginPersistedstate)

app.provide('container', container);

const vuetify = createVuetify({
    theme: {
        defaultTheme: 'light',
        themes: {
            dark: {
                colors: {
                    // "primary": colors.blue.darken3,
                    // "secondary": colors.cyan.accent1,
                    // "surface": colors.lightBlue.darken4
                }
            }
        }
    },
    icons: {
        defaultSet: 'mdi'
    },
    components,
    directives
});

app.use(vuetify);
app.use(pinia)
app.use(router)

app.mount('#app')
