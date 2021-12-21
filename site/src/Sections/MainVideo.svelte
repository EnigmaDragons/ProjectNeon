<script>
  import Container from '../Elements/Container.svelte';
  import Lazy from '../Elements/Lazy.svelte';
  import PlyrMp4Video from '../Elements/PlyrMp4Video.svelte';

</script>

<Container>
  <Lazy height={697} offset={50} name="video">
    <div class="center-vh py2">
      <h1>Game Trailer</h1>
      <div class="main-video">
        <PlyrMp4Video src="/video/Trailer.mp4" srcType="video/mp4" title="Cursed Gem - Trailer" poster="/video/trailer-poster.jpg"/>
      </div>
    </div>
  </Lazy>
</Container>

<style>
  h1 {
    font-size: 2.4em;
  }

  .main-video {
    width: 80%;
    position:relative;
  }

  .center-vh {
    display: flex;
    flex-direction: column;
    align-items: center;
    align-content: center;
    justify-items: center;
    justify-content: center;
  }
</style>
