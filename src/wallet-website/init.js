const { execSync } = require('child_process');
const readline = require('readline');
const fs = require('fs');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

console.log('\n🚀 Welcome to WalletPro Website Setup! 🚀\n');
console.log('This script will help you set up your project.\n');

// Function to execute commands and handle errors
function runCommand(command) {
  try {
    execSync(command, { stdio: 'inherit' });
    return true;
  } catch (error) {
    console.error(`Error executing command: ${command}`);
    console.error(error.message);
    return false;
  }
}

// Ask for project name
rl.question('Project name (default: walletpro-website): ', (projectName) => {
  projectName = projectName || 'walletpro-website';
  
  // Update package.json with the new project name
  const packageJson = JSON.parse(fs.readFileSync('./package.json', 'utf8'));
  packageJson.name = projectName;
  fs.writeFileSync('./package.json', JSON.stringify(packageJson, null, 2));
  
  console.log(`\nProject name set to: ${projectName}`);
  
  // Ask for author information
  rl.question('\nAuthor name: ', (authorName) => {
    if (authorName) {
      packageJson.author = authorName;
      fs.writeFileSync('./package.json', JSON.stringify(packageJson, null, 2));
      console.log(`Author set to: ${authorName}`);
    }
    
    console.log('\nInstalling dependencies...');
    if (runCommand('npm install')) {
      console.log('\n✅ Dependencies installed successfully!');
      
      console.log('\nRunning linters...');
      runCommand('npm run lint');
      runCommand('npm run stylelint');
      
      console.log('\n🎉 Setup completed successfully! 🎉');
      console.log('\nYou can now start the development server with:');
      console.log('npm start');
      
      console.log('\nTo build for production:');
      console.log('npm run build');
      
      rl.question('\nWould you like to start the development server now? (y/n): ', (answer) => {
        if (answer.toLowerCase() === 'y') {
          console.log('\nStarting development server...');
          runCommand('npm start');
        } else {
          console.log('\nThank you for using WalletPro Website Setup!');
          rl.close();
        }
      });
    } else {
      console.error('\n❌ Failed to install dependencies.');
      console.log('Please try running "npm install" manually.');
      rl.close();
    }
  });
}); 