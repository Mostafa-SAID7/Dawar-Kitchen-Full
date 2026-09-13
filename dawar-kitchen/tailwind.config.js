/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      fontFamily: {
        forum: ['Forum', 'serif'],
        sans: ['Open Sans', 'sans-serif'],
        cairo: ['Cairo', 'sans-serif'],
      },
      colors: {
        primary: '#C65A1E',
      },
    },
  },
  plugins: [
    // Enable RTL variant support
    function ({ addVariant, e }) {
      addVariant('rtl', [
        "@supports (direction: rtl) { [dir='rtl'] &",
        "@supports (direction: rtl) { .rtl &"
      ]);
      addVariant('ltr', [
        "@supports (direction: ltr) { [dir='ltr'] &",
        "@supports (direction: ltr) { .ltr &"
      ]);
    }
  ],
}

