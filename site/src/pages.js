import Home from './Pages/Home.svelte';
import Contact from './Pages/Contact.svelte';
import ContentCreatorsLicense from './Pages/ContentCreatorsLicense.svelte';
import PressKit from './Pages/PressKit.svelte';
import World from './Pages/World.svelte';
import Stories from './Stories/StoryMenu.svelte';
import WelcomeToZantoCorp from './Stories/WelcomeToZantoCorp.svelte';
import LegacyOfVopvanaTech from './Stories/LegacyOfVopvanaTech.svelte';
import TheHeist from './Stories/TheHeist.svelte';
import UnusualDayAstroVerse from './Stories/UnusualDayInTheAstroVerse.svelte';

const DefaultPage = Home;
export const pages = [
  { path: '/', href: '/', name: 'Home', component: DefaultPage, showInMainNav: true, useDefaultLayout: true },
  { path: '/world', href: '/index.html?page=world', name: 'World', component: World, showInMainNav: true, useDefaultLayout: true },
  { path: '/contact', href: '/index.html?page=contact', name: 'Contact', component: Contact, showInMainNav: true, useDefaultLayout: true },
  { path: '/contentcreators', href: '/index.html?page=contentcreators', name: 'Content Creators', component: ContentCreatorsLicense, showInMainNav: false, useDefaultLayout: true },
  { path: '/presskit', href: '/index.html?page=presskit', name: 'Press Kit', component: PressKit, showInMainNav: true, useDefaultLayout: true },
  { path: '/stories', href: '/index.html?page=stories', name: 'Stories', component: Stories, showInMainNav: false, useDefaultLayout: true },
  { path: '/story-welcome-to-zantocorp', href: '/index.html?page=story-welcome-to-zantocorp', name: 'Story - Welcome To ZantoCorp', component: WelcomeToZantoCorp, showInMainNav: false, useDefaultLayout: false },
  { path: '/story-vopvana', href: '/index.html?page=story-vopvana', name: 'Story - Legacy Of Vopvana', component: LegacyOfVopvanaTech, showInMainNav: false, useDefaultLayout: false },
  { path: '/story-the-heist', href: '/index.html?page=story-the-heist', name: 'Story - The Heist', component: TheHeist, showInMainNav: false, useDefaultLayout: false },
  { path: '/story-unusual-day-astroverse', href: '/index.html?page=unusual-day-astroverse', name: 'Story - Unusual Day in the AstroVerse', component: UnusualDayAstroVerse, showInMainNav: false, useDefaultLayout: false },
]
