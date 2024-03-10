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
          <v-app-bar-nav-icon @click.stop="showDrawer = !showDrawer"></v-app-bar-nav-icon>
        </template>
      </v-app-bar>
      <v-navigation-drawer fullHeight="true" v-model="showDrawer">
        <v-list-item title="Home" :to="{path: '/'}"></v-list-item>
        <v-list-item title="Add statement" :to="{path: '/add-statement'}"></v-list-item>
      </v-navigation-drawer>
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

<script setup lang="ts">
import { RouterView } from 'vue-router'

let showDrawer = defineModel<boolean>({default: false});

</script>
