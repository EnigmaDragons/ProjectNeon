<script>
  export let title, author, source;
  import SvelteMarkdown from 'svelte-markdown';
  import company from '../static-content.js';
</script>

<div class="all">
  <div class="story-container">
    <a href="/index.html?page=stories">
      <div class="logo-box">
        <picture>
          <source srcset={company.logoMobile} media="(max-width: 600px)">
          <img src={company.logo} alt="{company.name} Logo" height="198" width="320">
        </picture>
      </div>
    </a>

    <h1>{title}</h1>

    <h3 class="mb2">A Metroplex Zero Short Story</h3>

    {#if author}
      <h3>by {author.name}</h3>
    {/if}

    <div class="mb2"/>

    <SvelteMarkdown {source} />

    {#if author}
      <div class="mb2"/>
      <hr/>
      <div class="row author-box-bottom-padding">
        {#if author.img}
        <div class="img-box">
          <img src={author.img} alt={`Picture of ${author.name}`} /> 
        </div>
        {/if}
        <div class="column">
          <h2>Story by {author.name}</h2>
          {@html author.bioHtml}
        </div>
      </div>
      <hr/>
    {/if}
  </div>
</div>

<style>
  html {
    background-image: linear-gradient(rgba(0, 0, 0, 0), rgba(0, 0, 0, 0)), url("/images/bg-gradient-high-rez.jpg") !important;
  }

  .img-box {
    min-width: 120px;
    width: 320px;
    max-width: 320px;
    margin: auto 24px auto 12px;
  }

  .author-box-bottom-padding {
    margin-bottom: 24px;
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

  .mb2 {
    margin-bottom: 2em;
  }

  .story-container {
    padding-top: 4em;
    padding-bottom: 6em;
    margin-left: auto;
    margin-right: auto;
    width:80%;
    max-width: 800px;
    min-width: 320px;
  }

  .row {
    display: flex;
    flex-direction: row;
    align-content: center;
    justify-content: center;
  }
</style>