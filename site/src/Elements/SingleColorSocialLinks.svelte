<script>
  export let iconWidth = "1.2em";
  export let hideMobile = true;

  import company from '../static-content';
</script>

<div class={hideMobile ? "social-links mobile-hidden" : "social-link"}>
  {#if company.social.soundcloud}
  <a class="social-icon soundcloud-orange" href={company.social.soundcloud} target=”_blank”>
    <img src="/social/soundcloud.svg" alt="Soundcloud Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.yelp}
  <a class="social-icon yelp-red" href={company.social.yelp} target=”_blank”>
    <img src="/social/yelp.svg" alt="Yelp Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.facebook}
  <a class="social-icon facebook-blue" href={company.social.facebook} target=”_blank”>
    <img src="/social/facebook.svg" alt="Facebook Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.twitter}
  <a class="social-icon twitter-blue" href={company.social.twitter} target=”_blank”>
    <img src="/social/twitter.svg" alt="Twitter Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.reddit}
  <a class="social-icon reddit-orange" href={company.social.reddit} target=”_blank”>
    <img src="/social/reddit.svg" alt="Reddit Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.twitch}
  <a class="social-icon twitch-purple" href={company.social.twitch} target=”_blank”>
    <img src="/social/twitch.svg" alt="Twitch Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.steam}
  <a class="social-icon steam-blue" href={company.social.steam} target=”_blank”>
    <img src="/social/steam.svg" alt="Steam Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
  {#if company.social.itchio}
  <a class="social-icon itch-orange" href={company.social.steam} target=”_blank”>
    <img src="/social/itchio.svg" alt="Itch.io Icon" loading="lazy" style="width: {iconWidth}; height: auto;"/>
  </a>
  {/if}
</div>

<style>
  .social-links {
    margin: 10px;
    display: flex;
    flex-direction: row;
    background-color: transparent;
  }

  .social-icon {
    margin: 0 1em;
  }

  .social-icon img {
    filter: invert(.5) brightness(3);
  }

  .social-icon img:hover {
    filter: invert(.5) brightness(1.8);
  }

  .twitter-blue img:hover {
    filter: brightness(0.5) sepia(1) hue-rotate(140deg) saturate(6);
  }

  .facebook-blue img:hover {
    filter: brightness(0.3) sepia(1) hue-rotate(180deg) saturate(5);
  }

  .yelp-red img:hover {
    filter: brightness(0.4) sepia(1) hue-rotate(300deg) saturate(8);
  }

  .soundcloud-orange img:hover {
    filter: invert(47%) sepia(29%) saturate(3021%) hue-rotate(346deg) brightness(104%) contrast(101%);
  }

  .twitch-purple img:hover {
    filter: invert(33%) sepia(61%) saturate(4460%) hue-rotate(249deg) brightness(100%) contrast(101%);
  }

  .steam-blue img:hover {
    filter: invert(17%) sepia(61%) saturate(1771%) hue-rotate(189deg) brightness(98%) contrast(92%);
  }

  .itch-orange img:hover {
    filter: invert(60%) sepia(49%) saturate(4330%) hue-rotate(324deg) brightness(99%) contrast(97%);
  }

  .reddit-orange img:hover {
    filter: invert(34%) sepia(80%) saturate(4444%) hue-rotate(4deg) brightness(106%) contrast(102%);
  }
</style>
