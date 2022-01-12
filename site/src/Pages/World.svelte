<script>
  import Container from '../Elements/Container.svelte';
  import ICC from '../Lore/ICC.svelte';
  import AllMegaCorps from '../Lore/AllMegaCorps.svelte';
  import AboutMetroplexZero from '../Lore/AboutMetroplexZero.svelte';

</script>

<Container>
  <h1>The World of Metroplex Zero</h1>
  
  <AboutMetroplexZero />

  <AllMegaCorps />

  <h1>Stories and Lore</h1>

  <h2>Coming Soon!</h2>
  <!-- <div class="story-container">
    <ICC/>
  </div> -->
</Container>

<style>
  h1 {
    width: 100%;
    text-align: center;
  }

  .story-container {
    margin-left: auto;
    margin-right: auto;
    width:80%;
    max-width: 800px;
    min-width: 320px;
  }
</style>
