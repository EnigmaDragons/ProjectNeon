import svelte from 'rollup-plugin-svelte';
import resolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import livereload from 'rollup-plugin-livereload';
import { terser } from 'rollup-plugin-terser';
import serve from "rollup-plugin-serve";
import copy from "rollup-plugin-copy";

const production = !process.env.ROLLUP_WATCH;

export default {
	input: 'src/main.js',
	output: {
		sourcemap: !production,
		format: 'iife',
		name: 'app',
		file: 'public/build/bundle.[hash].js'
	},
	plugins: [

		svelte({
			dev: !production,
			css: css => {
				css.write("public/build/bundle.[hash].css", !production);
			},
		}),

		resolve({
			browser: true,
			dedupe: ['svelte']
		}),

		commonjs(),

		copy({
      targets: [{ src: 'static/g', dest: 'public' }],
    }),

		!production && serve({
			contentBase: ["public"],
			port: 5000,
		}),,
		!production && livereload({ watch: "public" }),
		production && terser()
	],
	watch: {
		clearScreen: false
	}
};
