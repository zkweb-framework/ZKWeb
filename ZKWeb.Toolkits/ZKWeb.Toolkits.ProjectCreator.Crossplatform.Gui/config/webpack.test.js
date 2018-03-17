var helpers = require("./helpers");

module.exports = {
    devtool: "inline-source-map",

    resolve: {
        extensions: [".ts", ".js"]
    },

    externals: { sqlite3: "commonjs sqlite3" },

    module: {
        loaders: [
            {
                test: /\.ts$/,
                loaders: ["awesome-typescript-loader", "angular2-template-loader"]
            },
            {
                test: /\.html$/,
                loader: "html-loader"

            },
            {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                loader: "null"
            },
            {
                test: /\.css$/,
                include: helpers.root("src", "app"),
                loader: "raw-loader"
            }
        ]
    },

    target:"electron-renderer",
};
