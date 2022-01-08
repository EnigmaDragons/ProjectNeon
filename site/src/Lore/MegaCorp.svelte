<script>
  export let corpName;
  export let corpDescription;
  export let flip;
</script>

<div class="row">
  {#if flip}
  <div class="description center">
    <p>
      {@html corpDescription}
    </p>
  </div>
  {/if}
  <div class="brand center">
    <h2>{corpName}</h2>
    <div class="logo center">
      <img src={`/images/corps/${corpName}_Logo1.png`} alt={corpName + " Logo"}/>
    </div>
  </div>
  {#if !flip}
  <div class="description center">
    <p>
      {@html corpDescription}
    </p>
  </div>
  {/if}
</div>
<hr/>

<style>
  .row {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
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
    min-width: 300px;
    margin-left: 1em;
    margin-right: 1em;
    width: 36vw;
  }

  hr {
    width: 100%;
    margin-top: 1.2em;
    margin-bottom: 1.2em;
  }
</style>