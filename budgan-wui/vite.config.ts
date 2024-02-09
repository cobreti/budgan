import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import sassDts from 'vite-plugin-sass-dts'
import tsconfigPaths from 'vite-tsconfig-paths';

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 8000,
    host: '0.0.0.0'
  },
  plugins: [
    vue(),
    sassDts(),
    vueJsx(),
    tsconfigPaths()
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '@XmlParser': fileURLToPath(new URL('./src/modules/XmlParser', import.meta.url)),
      '@models': fileURLToPath( new URL('./src/models', import.meta.url)),
    }
  }
})
