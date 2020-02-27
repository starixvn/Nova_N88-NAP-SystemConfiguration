const helpers = require("./helpers");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

let config = {
    entry: {
        "main": helpers.root("/src/main.ts")
    },
    output: {
        path: helpers.root("/dist"),
        filename: "[name].js"
    },
    devtool: "source-map",
    resolve: {
        extensions: [".ts", ".js", ".html"],
        alias: {
            'vue$': 'vue/dist/vue.common.js'
        }
    },
    module: {
        rules: [
        {
            test: /\.css$/,
            use: ExtractTextPlugin.extract({
                fallback: "style-loader",
                use: "css-loader"
            })
        },
        { test: /\.ts$/, exclude: /node_modules/, enforce: 'pre', loader: 'tslint-loader' },
        { test: /\.ts$/, exclude: /node_modules/, loader: "awesome-typescript-loader" },
        { test: /\.html$/, loader: 'raw-loader', exclude: ['./src/index.html'] },
        { test: /\.(eot|svg|ttf|woff|woff2)(\?\S*)?$/, loader: 'url-loader' },
        { test: /\.(png|jpe?g|gif|svg)(\?\S*)?$/, loader: 'file-loader', query: { name: '[name].[ext]?[hash]' } }
        ],
    },
    plugins: [
      new ExtractTextPlugin("css/main.css")
    ]
};

module.exports = config;