<script>
  export let pageName;

  let sidebarOpen = false;

  import HamburgerIcon from '../Elements/HamburgerIcon.svelte';
  import SiteSidebar from './SiteSidebar.svelte';

  import company from '../static-content';
  import { pages } from '../pages.js';
</script>

<header class="">
  <nav class="navbar-left">
    <a href="." class="logo">
      <div class="logo-img">
        <img class='no-filter' src={company.logo} alt="Logo"/>
      </div>
    </a>
    <a class="steam-link" href={company.social.steam}>
      <img src="/social/steam.svg" alt="Steam Icon" width=38 height=38/>
      Steam
    </a>
    {#if company.social.itchio}
    <a class="itch-link" href={company.social.itchio}>
      <img src="/social/itchio.svg" alt="Itch Icon" width=38 height=38/>
      itch.io
    </a>
    {/if}
  </nav>
  <SiteSidebar bind:open={sidebarOpen} />
  <nav class="navbar-right tablet-hidden">
    {#each pages.filter(p => !!p.showInMainNav && (!pageName || p.name !== pageName)) as page}
      <a class="navlink decor-font" href={page.href}>{page.name}</a>
    {/each}
    {#if company.social.twitter}
      <a class="twitter-link mobile-hidden" href={company.social.twitter}>
        <img src="/social/twitter-multi-color.png" alt="Twitter Icon"/>
      </a>
    {/if}
    {#if company.social.reddit}
      <a class="reddit-link mobile-hidden" href={company.social.reddit}>
        <img src="/social/reddit-multi-color.png" alt="Reddit Icon"/>
      </a>
    {/if}
  </nav>
  <HamburgerIcon bind:open={sidebarOpen}/>
</header>

<style>
  header {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    align-content: center;
    box-sizing: content-box;
    background-color: rgba(0, 0, 0, 0.4);
    height: 60px;
    z-index: 1;
  }

  nav {
    display: flex;
    flex-direction: row;
  }

  a {
    display: flex;
    flex-direction: row;
    justify-items: center;
    align-items: center;
    height: 60px;
    color: var(--header-link-color);
    padding: 0 0.8em;
    font-family: 'Body';
  }

  a:hover {
    background-color: #203c5a;
  }

  a:active {
    background-color: #3387a0;
  }

  .no-filter {
    filter: none !important;
  }

  .logo-img img {
    margin-top: 8px;
    width: 100px;
  }

  a img {
    width: 38px;

    filter: invert(0.8);
  }

  a img:last-of-type {
    margin-right: 0.5em;
  }

  .home {
    display: flex;
    justify-content: center;
  }

  h1 {
    margin: 10px;
  }

  .steam-link {
    letter-spacing: 0.05em;
  }

  .navlink {
    color: var(--header-link-color);
    padding: 0 1.2em;
    font-size: 1.2em;
    line-height: 22px;
    font-weight: 500;
    letter-spacing: 1.5px;
    text-transform: uppercase;
    transition: all 350ms;
    text-align: center;
  }

  .twitter-link img {
    filter: none;
  }

  .twitter-link:hover {
    background-color: #203c5a;
  }

  .twitter-link:active {
    background-color: #565656;
  }

  .reddit-link img {
    filter: none;
  }

  .reddit-link:hover {
    background-color: #343434;
  }

  .reddit-link:active {
    background-color: #565656;
  }

</style>
