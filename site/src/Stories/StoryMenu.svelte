<script>
  import Container from '../Elements/Container.svelte';
</script>

<Container>
  <div class="column center-vh">
    <h1>Stories of Metroplex Zero</h1>
    <a href="/index.html?page=story-welcome-to-zantocorp">1. Welcome To ZantoCorp</a>
    <a href="/index.html?page=story-vopvana">2. Legacy of Vopvana Tech</a>
  </div>
</Container>

<style>
  .column {
    display: flex;
    flex-direction: column;
  }

  a {
    font-size: 1.4em;
    margin-bottom: 0.2em;
  }
</style>