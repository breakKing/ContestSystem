let path = require('path');
let webpack = require('webpack');
let fs = require('fs');
let glob = require('glob');
const BrowserSyncPlugin = require('browser-sync-webpack-plugin')
const {VueLoaderPlugin} = require('vue-loader')
const MiniCssExtractPlugin = require('mini-css-extract-plugin');


let appBasePath = './ClientScripts/packs/'; // where the source files located
let publicPath = '../bundle/'; // public path to modify asset urls. eg: '../bundle' => 'www.example.com/bundle/main.js'
let bundleExportPath = './wwwroot/bundle/'; // directory to export build files

let jsEntries = {}; // listing to compile

glob.sync(appBasePath + '/**/*.js*').forEach(function (fpath) {
    if (fs.existsSync(fpath)) {
        let rel_fpath = fpath.replace(appBasePath, '')
        let entry_name = rel_fpath.substring(0, rel_fpath.lastIndexOf('.'))
        jsEntries[entry_name] = fpath
    }
})


module.exports = {
    entry: jsEntries,
    output: {
        path: path.resolve(__dirname, bundleExportPath),
        publicPath: publicPath,
        filename: '[name].js'
    },
    resolve: {
        extensions: ['.js', '.vue'],
        alias: {
            'vue$': 'vue/dist/vue.esm-browser.js',
            '@': path.join(__dirname, appBasePath)
        }
    },
    plugins: [
        new VueLoaderPlugin(),
        new webpack.HotModuleReplacementPlugin(),
        // new MiniCssExtractPlugin(),
        new BrowserSyncPlugin({
                // browse to http://localhost:3000/ during development,
                // ./public directory is being served
                // host: 'localhost',
                // port: 3000,
                proxy: 'http://localhost:5000',
                notify: false,
                // server: {baseDir: [bundleExportPath]}
            },
            // plugin options
            {
                injectCss: true,
                reload: false
            }
        )
    ],
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.js?$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env']
                    }
                }
            },
            {
                test: /\.s[ac]ss$/,
                use: ['style-loader', 'css-loader', 'sass-loader']
                // use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader']
            },
            {
                test: /\.css$/,
                // use: [MiniCssExtractPlugin.loader, 'css-loader'],
                use: ['style-loader', 'css-loader'],
            },
            {
                test: /\.(eot|svg|ttf|woff|woff2)(\?\S*)?$/,
                loader: 'file-loader'
            },
            {
                test: /\.(png|jpe?g|gif|svg)(\?\S*)?$/,
                loader: 'file-loader',
            }
        ]
    },
    devtool: 'source-map', //'#eval-source-map'
    devServer: {
        hot: true,
    }
}
module.exports.watch = process.env.WATCH === "true";
if (process.env.NODE_ENV === 'production') {
    module.exports.devtool = '#source-map'
    module.exports.plugins = (module.exports.plugins || []).concat([
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        }),
    ]);
} else if (process.env.NODE_ENV === "dev") {
    module.exports.watch = true;
    module.exports.plugins = (module.exports.plugins || []).concat([
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"development"'
            }
        }),
    ]);
}

