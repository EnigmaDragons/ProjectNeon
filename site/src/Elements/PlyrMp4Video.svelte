<script>
  export let poster;
  export let src;
  export let srcType = 'video/mp4';
  export let title = '';

  const onScriptLoaded = () => 
    window.player = new Plyr('#video-player', {
      title: title,
      ratio: '16:9'
    });

</script>

<svelte:head>
  <link rel="stylesheet" href="https://cdn.plyr.io/3.6.2/plyr.css" />
  <script src="https://cdn.plyr.io/3.6.2/plyr.js" on:load={onScriptLoaded} defer></script>
</svelte:head>

<video id="video-player" crossorigin playsinline controls data-poster={poster}>
  <source {src} type={srcType} />
</video>
