<script>
  import Container from './Container.svelte';
</script>

<Container>
  <div class="row">
    <div class="column">
      <slot name="first" />
    </div>
    <div class="column">
      <slot name="second" />
    </div>
  </div>
</Container>

<style>
  .row {
    display: flex;
    flex-direction: row;
    flex-wrap: no-wrap;
    justify-content: space-evenly;
    margin: 0 0 0 0;
    padding-bottom: 2em;
  }

  .column {
    max-width: 46vw;
    width: 40vw;
    display: flex;
    flex-direction: column;
    margin: 0px 1vw;
  }
</style>