var webpackConfig = require('./webpack.test');

module.exports = function (config) {
    var _config = {
        basePath: '',

        frameworks: ['jasmine'],
        browserNoActivityTimeout: 0,

        files: [
            { pattern: './karma-dummy.js', watched: false }, // For some reason an empty file is required
            { pattern: './karma-test-shim.js', watched: false },
        ],

        preprocessors: {
            './karma-dummy.js': ['electron'],  // And dummy file must be preprocessed too
            './karma-test-shim.js': ['electron', 'webpack', 'sourcemap'],
        },

        webpack: webpackConfig,

        webpackMiddleware: {
            stats: 'errors-only'
        },

        webpackServer: {
            noInfo: true
        },

        reporters: ['spec'],
        specReporter: {
            maxLogLines: 3,         // limit number of lines logged per test
            suppressErrorSummary: true,  // do not print error summary
            suppressFailed: false,  // do not print information about failed tests
            suppressPassed: false,  // do not print information about passed tests
            suppressSkipped: false,  // do not print information about skipped tests
            showSpecTiming: false // print the time elapsed for each spec
        },

        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: false,
        browsers: ['Electron'],
        singleRun: true,
        client: {
            useIframe: false,
        },
    };

    config.set(_config);
};
