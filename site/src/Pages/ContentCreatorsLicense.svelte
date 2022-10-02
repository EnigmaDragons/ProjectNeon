<script>
  import game from '../static-content.js';
</script>

<section>
  <div class="center-vh column">
    <div class="logo-box">
      <img src={game.logo} alt={`${game.name} Logo`} />
    </div>

    <h2>Let's Play Monetization Permission</h2>

    <p>{game.siteOwner} gives permission to any user to monetize their videos when doing Let's Plays, Speedruns, and Reviews of {game.name}.</p>

    <p>Logos, heroes, and many other assets can be found in the <a href={game.assetKit}>Content Creator Asset Kit</a></p>

    <p>{game.siteOwner} retains the right to revoke this license at any time.</p>
  </div>
</section>

<style>
  section {
    min-height: 94vh;
    background-image: linear-gradient(rgba(23, 17, 26, 0.01) 0%, rgba(23, 17, 26, 0.6) 33%, rgba(23, 17, 26, 0.9) 100%);
    width: 100%;
  }

  .logo-box {
    width: 320px;
    margin-bottom: 3em;
  }

  .column {
    flex-direction: column;
    padding: 6em;
  }

  a {
    margin-top: 1em;
    margin-bottom: 1em;
  }
</style>
