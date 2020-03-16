var webpack = require('webpack'),
    baseWebpackConfig = require('./webpack.config.base'),
    DefinePlugin = require('webpack/lib/DefinePlugin'),
    SourceMapDevToolPlugin = require('webpack/lib/SourceMapDevToolPlugin'),
    env = require('../environment/dev.env'),
    merge = require('webpack-merge');
const helpers = require("./helpers");
var webpackConfig = merge(baseWebpackConfig, {
    plugins: [
        new SourceMapDevToolPlugin({
            filename: null, // if no value is provided the sourcemap is inlined
            test: /\.(ts|js)($|\?)/i
        }),
        new DefinePlugin({
            'process.env': env
        })
    ],
    devtool: 'inline-source-map'
});

delete webpackConfig.entry

module.exports = webpackConfig;