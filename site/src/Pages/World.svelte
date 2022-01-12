<script>
  import Container from '../Elements/Container.svelte';
  import ICC from '../Lore/ICC.svelte';
  import AllMegaCorps from '../Lore/AllMegaCorps.svelte';
  import AboutMetroplexZero from '../Lore/AboutMetroplexZero.svelte';

</script>

<Container>
  <h1>The World of Metroplex Zero</h1>
  <hr/>  

  <AboutMetroplexZero />
  <hr/>

  <AllMegaCorps />

  <h2>Legal Charter</h2>
  <div class="story-container">
    <ICC/>
  </div>
</Container>

<style>
  h1, h2 {
    width: 100%;
    text-align: center;
  }

  hr {
    width: 100%;
    margin-top: 1.2em;
    margin-bottom: 1.2em;
  }

  .story-container {
    margin-left: auto;
    margin-right: auto;
    width:80%;
    max-width: 800px;
    min-width: 320px;
  }
</style>
