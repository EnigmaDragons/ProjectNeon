<script>
  import MegaCorp from "./MegaCorp.svelte";
  import Container from '../Elements/Container.svelte';

  const corps = [
    {corpName: 'Astropoly', desc: ''},
    {corpName: 'Medigeneix', desc: ''},
    {corpName: 'Novoline', desc: ''},
    {corpName: 'Plasmigarchy', desc: ''},
    {corpName: 'Secubotics', desc: ''},
    {corpName: 'Tritoonico', desc: ''},
    {corpName: 'ZantoCorp', desc: ''},
    {corpName: 'Unaffiliated', desc: ''},
  ]
</script>

<Container>
  <h1>MegaCorps of Metroplex Zero</h1>
	{#each corps as corp, i}
    <MegaCorp corpName={corp.corpName} corpDescription={corp.desc} flip={i % 2 === 1}/>
	{/each}
</Container>

<style>
  h1 {
    width: 100%;
    text-align: center;
  }
</style>