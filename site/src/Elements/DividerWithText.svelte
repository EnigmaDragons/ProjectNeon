<script>
  export let text;
</script>

<div class="bg-gradient">
  <div class="divider center fixed no-bg-color">
    <hr/>
    <div class="row no-bg-color">
      <h2>{text}</h2>
    </div>
    <hr/>
  </div>
</div>

<style>
  .bg-gradient {
    width: 100%;
  }

  h2 {
    white-space: nowrap;
    font-size: 2em;
  }

  .row {
    display: flex;
    flex-direction: row;
    flex-flow: row;
    justify-content: center;
    justify-items: center;
    align-content: center;
    align-items: center;
  }

  .divider {
    display: flex;
    flex-direction: row;
    flex-flow: row;
    justify-content: center;
    justify-items: center;
    align-content: center;
    align-items: center;
    width: 100%;
    max-width: 80vw;
    margin: 0px auto;
  }

  .divider > hr {
    width: 100%;
    border: 0;
    height: 1px;
    background: var(--primary-color-light);
  }

  .divider.center >:first-child {
      margin-right: 20px;
  }

  .divider.center >:last-child {
      margin-left: 20px;
  }

  @media screen and (max-width: 600px) {
    h2 {
      font-size: 1.6em;
    }

    svg {
      height: 2.5em;
      width: 2.5em;
    }

    .divider.center >:first-child {
      margin-right: 10px;
    }

    .divider.center >:last-child {
      margin-left: 10px;
    }
  }
</style>