const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

// Create dist directory if it doesn't exist
if (!fs.existsSync('dist')) {
  fs.mkdirSync('dist');
}

// Copy HTML, CSS, JS, and image files to dist
const filesToCopy = [
  'index.html',
  'styles.css',
  'script.js',
  'logo.svg',
  'wallet-app.png',
  'benefits.png'
];

filesToCopy.forEach(file => {
  if (fs.existsSync(file)) {
    fs.copyFileSync(file, path.join('dist', file));
    console.log(`Copied ${file} to dist/`);
  } else {
    console.warn(`Warning: ${file} not found`);
  }
});

// Create a simple version file with build timestamp
const buildInfo = {
  version: require('./package.json').version,
  buildDate: new Date().toISOString(),
  buildTimestamp: Date.now()
};

fs.writeFileSync(
  path.join('dist', 'build-info.json'),
  JSON.stringify(buildInfo, null, 2)
);
console.log('Created build-info.json');

// Log success message
console.log('\nBuild completed successfully!');
console.log('The optimized website is available in the dist/ directory');
console.log('To test the production build, you can run:');
console.log('npx serve dist'); 