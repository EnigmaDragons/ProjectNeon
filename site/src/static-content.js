export const gameDetails = ({
  features: [
    "Over 250 different cards, allowing for a blend of various playstyles and builds",
    "Over 70 unique game-changing augments",
    "9 heroes each with very different gameplay",
    "Level up your heroes multiple times in every run",
    "Choose your own level up perks every time you gain enough XP",
    "More than 20 unique random events",
    "Over 30 different enemies"
  ],
  shortDescription: "Cyberpunk party-based strategic card battle RPG. Assemble a team that fits your playstyle to take on evil megacorporations. Customize your decks and choose your cards wisely to defeat tough foes. Power up your heroes with gear, augments, cards, and level ups.",
  descriptionParagraphs: [
    "In 2280, Eurasica is ruled by cutthroat hyper-capitalist megacorporations. Only you can thwart ZantoCorp's attempts to reestablish a dark tyranny. Metroplex Zero brings a new take on roguelike deckbuilding with it's party-based RPG-style combat.",
    "To resist capitalistic tyranny, you’ll need to power up. Choose your route carefully, different locations give different benefits; upgrade your champion, recruit powerful units, upgrade cards, gain passive bonuses or duplicate any card in your deck.",
    "With 9 heroes to choose from, each has its own unique and surprising gameplay. Before each battle, scout your enemies and pick the ideal 36 cards to take on your foes. You are never forced to bring any card you don't like into battle. During your run you will be able to acquire new cards, equipment, and augments. You can get special surgical implants, level up your heroes, and manipulate corporations into giving you very nice shopping discounts.",
  ],
})

const site = ({
  name: 'Metroplex Zero',
  siteOwner: 'Enigma Dragons',
  owner: 'Enigma Dragons',
  ownerSite: 'https://www.enigmadragons.com',
  slogan: 'Strategic Cyberpunk Card Battler',
  email: 'games@enigmadragons.com',
  logo: './images/logo.png',
  logoMobile: './images/logo-small.png',
  publisherLogo: './images/logo.png',
  address: null,
  contactPrompt: 'Want an interview?<br>Got feedback on our game?<br>Send me a message',
  devLog: 'https://aikoncwd.itch.io/cursed-gem/devlog',
  social: {
    steam: 'https://store.steampowered.com/app/1412960/Metroplex_Zero/',
    twitter: 'https://twitter.com/EnigmaDragonsGS',
    //itchio: 'https://enigmadragons.itch.io',
    reddit: ''
  },
  screenshots: [
    '/images/img1.jpg',
    '/images/img2.jpg',
    '/images/img3.jpg',
    '/images/img4.jpg',
    '/images/img5.jpg',
    '/images/img6.jpg'
  ],
  gameDetailSections: [
  ]
});

export default site;

export const presskit = ({
  name: site.name,
  developer: site.siteOwner,
  location: 'Phoenix, AZ, USA',
  releaseDate: 'May 22, 2022',
  platforms: 'PC',
  website: 'https://www.metroplexzero.com',
  contact: site.email,
  social: site.social,
  pdf: './download/presskit.pdf',
  logo: site.logo,
  features: gameDetails.features,
  descriptionParagraphs: gameDetails.descriptionParagraphs,
  credits: [
    { name: 'Silas Reinagel', role: 'Executive Producer' },
    { name: 'Silas Reinagel', role: 'Game Design' },
    { name: 'Noah Reinagel', role: 'Game Design' },
    { name: 'Caleb Reinagel', role: 'Game Design' },
    { name: 'Silas Reinagel', role: 'Programming' },
    { name: 'Noah Reinagel', role: 'Programming' },
    { name: 'Paulo Lobo', role: 'Programming' },
    { name: 'Jean-Alexander Nevskiy', role: 'Sound Design' },
    { name: 'Jean-Alexander Nevskiy', role: 'Composer' },
    { name: 'Ian Booms', role: 'Composer' },
    { name: 'Yuliia Seliukova', role: 'Character Art' },
    { name: 'Ludmila Sosa', role: 'Character Art' },
    { name: 'Mustafa Contractor', role: 'Quality Testing' },
    { name: 'Josiah Reinagel', role: 'Alpha Playtesting' },
    { name: 'Stephanie Reinagel', role: 'Alpha Playtesting' },
    { name: 'David Reinagel', role: 'Alpha Playtesting' },
    { name: 'Daniel Reinagel', role: 'Alpha Playtesting' },
  ]
});
