<script>
  export let title, author, source;
  import SvelteMarkdown from 'svelte-markdown';
  import company from '../static-content.js';

</script>

<div class="all">
  <div class="story-container">
    <div class="logo-box">
      <picture>
        <source srcset={company.logoMobile} media="(max-width: 600px)">
        <img src={company.logo} alt="{company.name} Logo" height="198" width="320">
      </picture>
    </div>

    <h1>{title}</h1>

    {#if author}
    <h2>{author}</h2>
    {/if}

    <h3 class="spacing">A Metroplex Zero Short Story</h3>

    <SvelteMarkdown {source} />
  </div>
</div>

<style>
  .all {
    background-image: linear-gradient(rgba(0, 0, 0, 0), rgba(0, 0, 0, 0)), url("/images/bg-gradient-high-rez.jpg") !important;
  }

  .logo-box {    
    margin-left: auto;
    margin-right: auto;
    width: 320px;
    min-height: 198px;
  }

  h1, h2, h3 {
    margin-left: auto;
    margin-right: auto;
    text-align: center;
  }

  h3 {
    font-size: 1em;
  }

  .spacing {
    margin-bottom: 4em;
  }

  .story-container {
    padding-top: 4em;
    padding-bottom: 8em;
    margin-left: auto;
    margin-right: auto;
    width:80%;
    max-width: 800px;
    min-width: 320px;
  }
</style>