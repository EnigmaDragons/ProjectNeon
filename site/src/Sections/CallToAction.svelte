<script>
  import project from '../static-content.js';
</script>

<a href={project.social.steam} class="center-vh center">
  <div class="icon-box">
    <img src="/social/steam.svg" loading="lazy" alt="Steam Link"/>
  </div>
  <h2>Get {project.name}</h2>
</a>

<style>
  .icon-box {
    width: 64px;
    height: 64px;
    margin-right: 1em;
    margin-left: 0.64em;
    margin-top: 0.64em;
  }

  img {
    filter: invert(1);
  }

  h2 {
    font-size: 1.2em;
    text-align: center;
  }

  a {
    background-color: var(--secondary-color);
    border-radius: 16px;
    padding: 0.6em 1.2em;
    transition-duration: 500ms;
    color: var(--primary-color);
    display: flex;
    width: 400px;
    max-width: 90vw;

    box-shadow: 0px 0px 30px 5px var(--secondary-color-press);
  }

  a:hover {
    background-color: var(--secondary-color-hover);
    color: var(--primary-color);

    box-shadow: 0px 0px 40px 20px var(--secondary-color-hover);
  }

  a:active {
    background-color: var(--seconary-color-press);
    color: var(--primary-color);

    box-shadow: 0px 0px 40px 0px var(--secondary-color);
  }
</style>