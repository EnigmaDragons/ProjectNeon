<script>
  export let corpName;
  export let corpDescription;
  export let flip;
</script>

<div class="row">
  {#if flip}
  <div class="description center-vh">
    <p>
      {corpDescription}
    </p>
  </div>
  {/if}
  <div class="brand">
    <h2 class="center">{corpName}</h2>
    <div class="logo center">
      <img src={`/images/corps/${corpName}_Logo1.png`} alt={corpName + " Logo"}/>
    </div>
  </div>
  {#if !flip}
  <div class="description center-vh">
    <p>
      {corpDescription}
    </p>
  </div>
  {/if}
</div>

<style>
  .row {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    margin-bottom: 2.4em;
  }

  .brand {
    min-width: 240px;
    width: 300px;
    max-width: 80vw;
  }

  h2 {
    font-size: 1.8em;
    text-align: center;
    width: 100%;
  }

  .logo {
    min-width: 240px;
    width: 300px;
    max-width: 80vw;
  }

  .description {
    padding-top: 4em;
    padding-left: 1em;
    width: 36vw;
  }
</style>