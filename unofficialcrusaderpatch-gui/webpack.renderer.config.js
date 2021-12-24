const rules = require('./webpack.rules');
const plugins = require('./webpack.renderer.plugins');
const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

rules.push(
  { 
    test: /\.ts$/, 
    use: 'ts-loader', exclude: /node_modules/ 
  },
  {
    test: /\.css$/,
    use: [
      { loader: 'style-loader' },
      { loader: 'css-loader' },
    ]
  },
  {
    test: /\.(scss)$/,
    use: [
      { loader: 'style-loader' },
      { loader: 'css-loader' },
      { loader: 'sass-loader' }, 
      { 
        loader: 'postcss-loader',   
        options: {
          postcssOptions: {
            plugins: function () {
              return [
                require('autoprefixer')
              ];
            }
          }
        },
      }
    ],
  },
  // {
  //   test: /\.(png|jpe?g|gif|ico|svg)$/,
  //   use: [
  //     {
  //       loader: "file-loader",
  //     }
  //   ]
  // },
  {
    test: /\.node$/,
    use: 'node-loader',
  },
  {
    test: /\.(m?js|node)$/,
    parser: { amd: false },
    use: {
      loader: '@marshallofsound/webpack-asset-relocator-loader',
      options: {
        outputAssetBase: 'native_modules',
      },
    },
  },
  {
    test: /\.m?js$/,
    exclude: /(node_modules)/,
    use: {
      loader: 'babel-loader',
      options: {
        presets: ['@babel/preset-env']
      }
    }
  }
);

module.exports = {
  module: {
    rules,
  },
  plugins: plugins,
  resolve: {
    extensions: ['.js', '.ts', '.jsx', '.tsx', '.css'],
    plugins: [new TsconfigPathsPlugin({})]
  },
};
