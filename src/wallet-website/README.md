# WalletPro - Wallet Application Website

A modern, responsive single-page website for a wallet application with attractive animations and a custom logo.

## Features

- **Responsive Design**: Fully responsive layout that works on all devices
- **Modern UI**: Clean and modern interface with gradient accents
- **Animations**: Smooth animations and transitions for enhanced user experience
- **Custom SVG Graphics**: Hand-crafted SVG illustrations and logo
- **Interactive Elements**: Testimonial slider, animated feature cards, and more

## Project Structure

- `index.html` - Main HTML file
- `styles.css` - CSS styles and animations
- `script.js` - JavaScript for interactivity and animations
- `logo.svg` - Custom SVG logo
- `wallet-app.png` - Illustration of the app interface
- `benefits.png` - Illustration for the benefits section
- `package.json` - NPM package configuration
- `.eslintrc.json` - ESLint configuration for JavaScript
- `.stylelintrc.json` - Stylelint configuration for CSS
- `build.js` - Custom build script for production
- `init.js` - Initialization script for project setup

## Technologies Used

- HTML5
- CSS3 (with modern features like CSS variables, flexbox, grid)
- JavaScript (ES6+)
- SVG for graphics and animations
- AOS (Animate On Scroll) library
- NPM for package management
- ESLint for JavaScript linting
- Stylelint for CSS linting

## Getting Started

### Prerequisites

- Node.js (v14 or higher)
- npm (v6 or higher)

### Installation

1. Clone or download this repository
2. Install dependencies:
   ```
   npm install
   ```
3. Run the initialization script:
   ```
   npm run init
   ```
   This script will:
   - Ask for your project name and author information
   - Install all dependencies
   - Run linters to check code quality
   - Offer to start the development server

### Development

Start the development server:
```
npm start
```

This will start a local development server at http://localhost:8080 with live reloading.

### Linting

Lint JavaScript files:
```
npm run lint
```

Fix JavaScript linting issues automatically:
```
npm run lint:fix
```

Lint CSS files:
```
npm run stylelint
```

Fix CSS linting issues automatically:
```
npm run stylelint:fix
```

Validate HTML:
```
npm run validate
```

Run all linting checks:
```
npm test
```

### Building for Production

Build the project for production:
```
npm run build
```

This will create a `dist` directory with all the necessary files for deployment.

To test the production build locally:
```
npm run serve:dist
```

## Customization

You can easily customize this template by:

- Changing the color scheme in the CSS variables (in `:root`)
- Replacing the SVG graphics with your own
- Modifying the content in the HTML file
- Adding more sections or features as needed

## Browser Support

This website works in all modern browsers including:
- Chrome
- Firefox
- Safari
- Edge

## License

This project is available for use under the MIT License.

## Credits

- Font Awesome for icons
- Google Fonts for typography
- AOS library for scroll animations
- Random User API for testimonial avatars 