<script>
  import TwoColumnLayout from '../Elements/TwoColumnMobileLayout.svelte';
  import Lazy from '../Elements/Lazy.svelte';
  import project from '../static-content';

  const screenshots = project.screenshots;

  const leftImgs = screenshots.filter((_,i) => i % 2 === 0);
  const rightImgs = screenshots.filter((_,i) => i&1);

  const skeletonHeight = leftImgs.length * 300;
</script>

<Lazy height={skeletonHeight}>
  <TwoColumnLayout>
    <div slot="first">
      {#each leftImgs as image}
        <a href={image}><div class='screenshot'><img  src={image} alt='Screenshot' lazy='true'/></div></a>
      {/each}
    </div>
    <div slot="second">
      {#each rightImgs as image}
        <a href={image}><div class='screenshot'><img  src={image} alt='Screenshot' lazy='true'/></div></a>
      {/each}
    </div>
  </TwoColumnLayout>
</Lazy>

<style>
  .screenshot {
    max-width: 42vw;
    margin: 2em 0em;
  }
</style>
