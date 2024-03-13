<template>
  <v-container class="d-flex flex-wrap align-content-start flex-row">
    <account-tile class="account-tile" v-for="t in tiles" :title="t"></account-tile>
  </v-container>
</template>

<style scoped>
  .account-tile {
    display: block;
    position: relative;
    min-height: 10em;
    max-height: 10em;
    min-width: 20em;
    width: 20em;
    margin: 2em;
  }
</style>

<script setup lang="ts">
import AccountTile from '@/components/AccountTile.vue';

const tiles: string[] = [];

for (let i = 0; i < 10; i++) {
  tiles.push(`Tile ${i}`);
}

</script>
