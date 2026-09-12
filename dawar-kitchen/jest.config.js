/** @type {import('jest').Config} */
module.exports = {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
  testEnvironment: 'jsdom',
  testMatch: ['**/src/**/*.spec.ts'],
  moduleFileExtensions: ['ts', 'js', 'mjs', 'html', 'json'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
  },
  transform: {
    '^.+\\.(ts|js|mjs|html|svg)$': [
      'jest-preset-angular',
      {
        tsconfig: '<rootDir>/tsconfig.spec.json',
        stringifyContentPathRegex: '\\.html$',
        diagnostics: false,
      },
    ],
  },
  transformIgnorePatterns: [
    'node_modules/(?!(@angular|rxjs|zone.js|@ngx-translate)/)',
  ],
  collectCoverage: true,
  coverageDirectory: '../frontend-coverage',
  coverageReporters: ['text', 'lcov', 'cobertura', 'json', 'json-summary'],
  collectCoverageFrom: [
    'src/app/services/**/*.ts',
    'src/app/components/**/*.ts',
    '!src/app/**/*.spec.ts',
    '!src/app/**/*.d.ts',
    '!src/app/**/tests/*.base.ts',
    '!src/app/**/tests/component-test.base.ts',
    '!src/app/**/tests/service-test.base.ts',
  ],
  coverageThreshold: {
    global: {
      statements: 65,
      branches: 50,
      functions: 65,
      lines: 65,
    },
  },
};
