/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['../**/*.{razor,html}'],
    theme: {
        extend: {
            colors: {
                'primaryBg': '#e3e5ea',
                'overlayBg': '#ffffff',
                'primary': {
                    100: '#e0e1fb',
                    200: '#6f7cef',
                },
                'secondary': {
                    100: '#E2E2D5',
                    200: '#888883',
                }
            },
        },
    },
    plugins: [],
}

