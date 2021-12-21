<script>
  import Container from '../Elements/Container.svelte';
  import Screenshots from '../Sections/Screenshots.svelte';

  import { presskit } from '../static-content.js';
</script>

<section role="main">
  <Container>

  <div class="column mt4 px1 press center">
    <h1 class="center">{presskit.name} Press Kit</h1>

    <div class="presskit-section facts">
      <hr>
      <h3>Fact Sheet</h3>
      <hr class="mb2">
      <p><b>Developer</b>: {presskit.developer}</p>
      <p><b>Location</b>: {presskit.location}</p>
      <p><b>Release Date</b>: {presskit.releaseDate}</p>
      <p><b>Platforms</b>: {presskit.platforms}</p>
      <p><b>Website</b>: <a href={presskit.website}>{presskit.website}</a></p>
      <p><b>Press Contact</b>: <a href="mailto:{presskit.contact}">{presskit.contact}</a></p>
      <p><b>Presskit (PDF)</b>: <a href={presskit.pdf}>Download</a></p>
      <p><b>Social</b>:
        {#if presskit.social.discord}
          <a href={presskit.social.discord}>Discord</a>
        {/if}
        {#if presskit.social.twitter}
          <a href={presskit.social.twitter}>Twitter</a>
        {/if}

      <div class="center logo-box">
        <img src="{presskit.logo}" alt="{presskit.name} Logo"/>
      </div>
    </div>
  ​
    <div class="presskit-section product">
      <hr>
      <h3>Project Description</h3>
      <hr>
      {#each presskit.descriptionParagraphs as paragraph}
  ​      <p>{paragraph}</p>
      {/each}
    </div>

    <div class="presskit-section features">
      <hr>
      <h3>Features</h3>
      <hr>
      {#each presskit.features as feature}
  ​     <p>● {feature}</p>
      {/each}
    </div>

    <div class="screenshots">
      <hr>
      <h3>Screenshots</h3>
      <hr>

      <Screenshots/>
    </div>

    <div class="presskit-section credits">
      <hr>
      <h3>Credits</h3>
      <hr>

      <div class='two-col'>
      {#each presskit.credits as credit}
        <div class='two-col-item'>
          <p class='credit'><b>{credit.name}</b>: {credit.role}</p>
        </div>
      {/each}
      </div>
    </div>

    <div class="presskit-section contact">
      <hr>
      <h3>Contact</h3>
      <hr>

      <p><b>Inquiries</b>: <a href="mailto:{presskit.contact}">{presskit.contact}</a></p>
      <p><b>Twitter</b>: <a href={presskit.social.twitter}>{presskit.social.twitter}</a></p>
      <p><b>Web</b>: <a href={presskit.website}>{presskit.website}</a>
    </div>
  </div>
  </Container>
</section>

<style>
  section, main {
    background-image: linear-gradient(rgba(23, 17, 26, 0.01) 0%, rgba(23, 17, 26, 0.9) 100%);
  }

  .two-col {    
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: space-between;
  }

  .two-col-item {
    width: 320px;
    min-width: 32vw;
  }

  .credit {
    margin: 4px 0;
  }

  .press {
    width: 80vw;
    max-width: 900px;
  }

  .logo-box {
    width: 36em;
    max-width: 80vw;
  }

  .center {
    margin-left: auto;
    margin-right: auto;
  }

  .features p {
    margin-top: -1em;
  }

  .product p {
    margin-top: -1em;
  }

  p {
    margin: 0.5em 0px;;
    line-height: 1.4em;
  }

  a {
    font-size: 1em;
  }

  .screenshots {
    background-image: rgba(23, 17, 26, 0.95);
  }

  .presskit-section {
    padding-bottom: 2em;
  }
</style>