import { cpSync, rmSync, watch } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const scriptDir = dirname(fileURLToPath(import.meta.url));
const src = join(scriptDir, '..', 'dist', 'budgan', 'browser');
const dest = join(scriptDir, '..', '..', 'server', 'BudganSvr', 'wwwroot');

function copy() {
  rmSync(dest, { recursive: true, force: true });
  cpSync(src, dest, { recursive: true });
  console.log(`Copied ${src} -> ${dest}`);
}

copy();

if (process.argv.includes('--watch')) {
  let timeout;
  watch(src, { recursive: true }, () => {
    clearTimeout(timeout);
    timeout = setTimeout(copy, 300);
  });
  console.log(`Watching ${src} for changes...`);
}
