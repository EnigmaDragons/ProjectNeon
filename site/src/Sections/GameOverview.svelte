<script>
  import Lazy from '../Elements/Lazy.svelte';
  import DividerWithText from '../Elements/DividerWithText.svelte';
  import project from '../static-content';

  const gameDetails = project.gameDetailSections;

  const skeletonHeight = (gameDetails.length / 2) * 150;
</script>

<div class="column center-vh">
  <div class="header-box">
    <DividerWithText text='Game Overview'/>
  </div>
  <Lazy height={skeletonHeight}>
      <div class="center-vh center zigzag">
        {#each gameDetails as g}
          {#if (!!g.image)}
            <a href={g.image}><div class='zigzag-item'><img  src={g.image} alt='Screenshot' lazy='true'/></div></a>
          {/if}
          {#if (!!g.text)}
            <div class="zigzag-item decor-font">{@html g.text}</div>
          {/if}
        {/each}
      </div>
  </Lazy>
</div>

<style>
  .column {
    display: flex;
    flex-direction: column;
  }

  .header-box {
    max-width: 92vw;
    width: 40em;
  }

  .zigzag {    
    flex-direction: row;
    flex-wrap: wrap;
    max-width: 92vw;
  }

  .zigzag-item {
    max-width: 92vw;
    width: 360px;
    min-width: 32vw;
    margin: 1em 12px;
    text-align: center;
    display: flex;
    justify-content: center;
    justify-items: center;
    align-content: center;
    align-items: center;
  }
</style>