const site = require('./metadata.js');
const fs = require("fs");
const replace = require('replace-in-file');

const replaceFs = (targetFile, from, to) => {
  try {
    let text = fs.readFileSync(targetFile, 'utf8');
    text = text.replace(from, to);
    fs.writeFileSync(targetFile, text);
  } catch(e) {
      console.log('Error:', e.stack);
      process.exit(-1);
  }
};

// Twitter Card

let twitterCard = "";
twitterCard = twitterCard.concat(`\t<meta name="twitter:card" content="summary_large_image" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:creator" content="@${site.twitterUsername}" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:site" content="@${site.twitterUsername}" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:title" content="${site.name}" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:description" content="${site.description}" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:url" content="${site.url}" />\n`);
twitterCard = twitterCard.concat(`\t<meta name="twitter:image" content="${site.image}" />`);

replaceFs('./public/index.html', '\t<!-- Twitter Card Slot -->', twitterCard);

// Open Graph

let openGraph = "";
openGraph = openGraph.concat(`\t<meta name="og:type" content="website" />\n`);
openGraph = openGraph.concat(`\t<meta name="og:title" content="${site.name}" />\n`);
openGraph = openGraph.concat(`\t<meta name="og:description" content="${site.description}" />\n`);
openGraph = openGraph.concat(`\t<meta name="og:url" content="${site.url}" />\n`);
openGraph = openGraph.concat(`\t<meta name="og:image" content="${site.image}" />\n`);

replaceFs('./public/index.html', '\t<!-- Open Graph Slot -->', openGraph);

// Metadata 

let metaDescription = `\t<meta name="description" content="${site.description}"/>\n`;
replaceFs('./public/index.html', '\t<!-- Meta Description Slot -->', metaDescription);

let metaKeywords = `\t<meta name="keywords" content="${site.keywords}"/>\n`;
replaceFs('./public/index.html', '\t<!-- Meta Keywords Slot -->', metaKeywords);

// Cache Busting

const randomString = (len, charSet) => {
  charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  var randomString = '';
  for (var i = 0; i < len; i++) {
      var char = Math.floor(Math.random() * charSet.length);
      randomString += charSet.substring(char, char + 1);
  }
  return randomString;
};

const hash = randomString(12);
fs.renameSync('public/build/bundle.[hash].css', `./public/build/bundle.${hash}.css`, e => { if ( e ) console.error('Error: ' + e); });
fs.renameSync('public/build/bundle.[hash].js', `./public/build/bundle.${hash}.js`, e => { if ( e ) console.error('Error: ' + e); });

const options = {
  files: './public/index.html',
  from: /\[hash\]/g,
  to: hash,
};

replace(options, (error, _) => {
  if (error) {
    return console.error('Error:', error);
  }
});

