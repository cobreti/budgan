<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div>
    <v-app full-height="100%">
      <v-app-bar color="teal-darken-4" image="https://picsum.photos/1920/1080?random">
        <template v-slot:image>
          <v-img
              gradient="to top right, rgba(19,84,122,.8), rgba(128,208,199,.8)"
          ></v-img>
        </template>
        <template v-slot:prepend>
          <v-app-bar-nav-icon></v-app-bar-nav-icon>
        </template>
      </v-app-bar>
      <v-main scrollable="false">
        <RouterView v-slot="{Component, route}">
          <transition :name="route.meta.transition || 'fade'">
            <component :is="Component" />
          </transition>
        </RouterView>
      </v-main>
    </v-app>
  </div>
</template>

<style scoped>

</style>
