var path = require("path");
var webpack = require("webpack");
var HtmlWebpackPlugin = require("html-webpack-plugin");
var ExtractTextPlugin = require("extract-text-webpack-plugin");
var CopyWebpackPlugin = require("copy-webpack-plugin");
var helpers = require("./helpers");

module.exports = {
    entry: {
        "polyfills": "./src/polyfills.ts",
        "vendor": "./src/vendor.ts",
        "app": "./src/main.ts"
    },

    resolve: {
        extensions: [".ts", ".js"]
    },

    externals: {
        sqlite3: "commonjs sqlite3",
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /\.spec\.ts$/,
                use: ["awesome-typescript-loader", "angular2-template-loader"]
            },
            {
                test: /\.html$/,
                use: [
                    {
                        loader: "html-loader"
                    },
                ]
            },
            {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                use: "file-loader?name=assets/images/[name].[ext]"
            },
			{
				test: /\.scss$/,
                use: [
                    {
                        loader: "style-loader"
                    },
                    {
                        loader: "css-loader"
                    },
                    {
                        loader: "postcss-loader", // Run post css actions
                        options: {
                            plugins: function () { // post css plugins, can be exported to postcss.config.js
                                return [
                                    require("precss"),
                                    require("autoprefixer")
                                ];
                            }
                        }
                    },
                    {
                        loader: "sass-loader",
                        options: {
                            includePaths: [
                                path.resolve(__dirname, "../src/assets/sass"),
                                path.resolve(__dirname, "../node_modules/bootstrap/scss"),
                            ]
                        }
                    }
                ]
			},
            {
                test: /\.css$/,
                exclude: helpers.root("src", "app"),
                use: ExtractTextPlugin.extract({ fallback: "style-loader", use: "css-loader" })
            },
            {
                test: /\.css$/,
                include: helpers.root("src", "app"),
                use: "raw-loader"
            }
        ]
    },

    plugins: [
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery",
            Popper: ["popper.js", "default"],
        }),
        new webpack.optimize.CommonsChunkPlugin({name: ["app", "vendor", "polyfills"]}),
        new HtmlWebpackPlugin({template: "src/index.html"}),
		new CopyWebpackPlugin([
            { 
                from: "src/assets", 
                to: "assets",
                toType: "dir" 
            },
        ]),
        new webpack.ContextReplacementPlugin(
            /angular(\\|\/)core(\\|\/)(@angular|esm5)/,
            path.resolve(__dirname, "../src")
        ),
    ],

    target:"electron-renderer",
    node: {
           __dirname: true
        },
};
