/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#1890ff',
          dark: '#096dd9',
          light: '#40a9ff',
        },
        secondary: {
          DEFAULT: '#722ed1',
          dark: '#531dab',
          light: '#9254de',
        },
      },
    },
  },
  plugins: [],
  corePlugins: {
    preflight: false, // For Ant Design compatibility
  },
} 