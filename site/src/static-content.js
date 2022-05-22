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
  shortDescription: "Cyberpunk party-based card battler. Assemble a team that fits your playstyle to take on evil MegaCorps. Customize your decks and choose your cards wisely to defeat tough foes. Power up your heroes with cards, implants, and augments. ",
  descriptionParagraphs: [
    "We fused card games, roguelikes and JRPGS together to make the best single player card battler we could. Assemble your squad, customize your decks, gain powerful cybernetic augments, and take on evil megacorporations!",
    "People have described Metroplex Zero as a cross between Slay the Spire, Magic: The Gathering, and Shadowrun.",
    "In 2280, Eurasica is ruled by cutthroat hyper-capitalist megacorporations. To resist capitalistic tyranny, you’ll need to power up. Choose your route carefully, different locations give different benefits; upgrade your champion, recruit powerful units, upgrade cards, gain passive bonuses or duplicate any card in your deck.",
    "There are a variety of unique heroes you can include in your squad, each with their own unique and surprising gameplay. Every hero has a unique combination of skills. Want to throw grenades and shoot rifles? Protect your team with powered shields? Interfere with enemy plans? Afflict your foes with intense psychological pain? Hide in the shadows and wait for the perfect time to strike? You can do all of that! Put the heroes you want in your squad and synergize their skills for double-infinite variety of playstyles!",
    "Gain powerful cards for your heroes by winning battles, or purchasing training from the VR skills training shop. Customize your deck to develop your own squad strategy, empowering each hero to do what you want them to do, and preparing them to deal with a variety of challenging foes. You are never forced to bring any card you don't like into battle. Under your strategic direction, each of your heroes will bring the perfect 12 cards into battle. Before each battle, scout your enemies and pick the ideal cards to take them on.",
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
    itchio: 'https://enigmadragons.itch.io',
    reddit: '',
    discord: 'https://discord.gg/V3yKWAwknC',
  },
  screenshots: [
    '/images/1.jpg',
    '/images/2.jpg',
    '/images/3.jpg',
    '/images/4.jpg',
    '/images/5.jpg',
    '/images/6.jpg',
    '/images/7.jpg',
    '/images/8.jpg',
  ],
  gameDetailSections: [
  ]
});

export default site;

export const presskit = ({
  lazy: true,
  name: site.name,
  developer: site.siteOwner,
  location: 'Phoenix, AZ, USA',
  genre: 'Card Battler, Roguelike, Strategy, RPG',
  releaseDate: 'Q3, 2022',
  platforms: 'Steam - PC, Steam - Mac',
  website: 'https://www.metroplexzero.com',
  contact: site.email,
  social: site.social,
  pdf: './download/presskit.pdf',
  logo: site.logo,
  price: "$19.99",
  features: gameDetails.features,
  descriptionParagraphs: gameDetails.descriptionParagraphs,
  trailer: 'https://www.youtube-nocookie.com/embed/L-PdE4qIUok',
  credits: [
    { name: 'Silas Reinagel', role: 'Executive Producer' },
    { name: 'Silas Reinagel', role: 'Game Design' },
    { name: 'Noah Reinagel', role: 'Game Design' },
    { name: 'Caleb Reinagel', role: 'Game Design' },
    { name: 'Silas Reinagel', role: 'Programming' },
    { name: 'Noah Reinagel', role: 'Programming' },
    { name: 'Paulo Lobo', role: 'Programming' },
    { name: 'Jean-Alexander Nevskiy', role: 'Sound Design' },
    { name: 'Tony Vilgotsky', role: 'Sound Design' },
    { name: 'Jean-Alexander Nevskiy', role: 'Composer' },
    { name: 'Ian Booms', role: 'Composer' },
    { name: 'Ants Aare Alamaa', role: 'Environment Art' },
    { name: 'Yuliia Seliukova', role: 'Character Art' },
    { name: 'Ludmila Sosa', role: 'Character Art' },
    { name: 'Mustafa Contractor', role: 'Quality Testing' },
    { name: 'Josiah Reinagel', role: 'Alpha Playtesting' },
    { name: 'Stephanie Reinagel', role: 'Alpha Playtesting' },
    { name: 'David Reinagel', role: 'Alpha Playtesting' },
    { name: 'Daniel Reinagel', role: 'Alpha Playtesting' },
  ]
});
