import './assets/main.css'

import { createApp } from 'vue'
import { createVuetify } from 'vuetify';
import { createPinia } from 'pinia'
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import colors from 'vuetify/util/colors';
import 'vuetify/styles';

import App from './App.vue'
import router from './router'

const app = createApp(App)

const vuetify = createVuetify({
    theme: {
        defaultTheme: 'dark',
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
    components,
    directives
});

app.use(vuetify);
app.use(createPinia())
app.use(router)

app.mount('#app')
