<script>
  import Container from '../Elements/Container.svelte';
  import ICC from '../Lore/ICC.svelte';
  import AllMegaCorps from '../Lore/AllMegaCorps.svelte';

</script>

<Container>
  <AllMegaCorps />

  <h1>Stories and Lore</h1>

  <div class="story-container">
    <ICC/>
  </div>
</Container>

<style>
  .story-container {
    margin-left: auto;
    margin-right: auto;
    width:80%;
    max-width: 800px;
    min-width: 320px;
  }
</style>
