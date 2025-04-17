export default { 
    testEnvironment: 'jest-environment-jsdom',
    transform: {
        '^.+\\.(js|jsx)$': 'babel-jest', // ✅ Ensures JSX is passed to Babel
    },
    moduleFileExtensions: ['js', 'jsx'],
    setupFilesAfterEnv: ['<rootDir>/src/setupTests.js'],
};

