<script>
  import MegaCorp from "./MegaCorp.svelte";
  import Container from '../Elements/Container.svelte';

  const corps = [
    {corpName: 'Astropoly', desc: 'Kings of commerce. If you want to buy or sell, Astropoly has you covered.<br><br>Astropoly are reselling masters who boast the largest selection of cutting-edge merchandise. While most of the other corporations are trying to outcompete each other with corporate espionage, proprietary product innovations and cutthroat business practices, Astropoly is cutting deals at nice discount margins so they can be the ones selling the latest tech to consumers.<br><br>They are friendly, with the best customer service in the business. On the backend, Astropoly uses robot staffed warehouses and employs relatively few workers. Astropoly purchase kiosks can be found everywhere in the city, and they offer premier metaverse shopping experiences that are lightyears ahead of their competitors.<br><br>Lest you think they are solely held in positive regard, there are plenty of rumors that Astropoly is involved in reverse engineering, product counterfeiting, privacy invasion, psychological manipulation, and unethical advertising practices. To date, no one has been able to produce sufficient public evidence for any of these claims, so it may just be corporate jealousy...'},

    {corpName: 'Medigeneix', desc: 'Gene therapy and body specialists, helping you stimulate, decompress, overperform, relax, and detoxify.<br><br>Medigeneix has taken allopathic medicine into new realms. Not only are they experts at treating disease and performing life-saving surgeries, but they also specialize in all kinds of interventionalist enhancements. They manufacture a broad array of biogenetic augments to improve virtually any physical attribute you want. These are extremely popular for benefactors with highly physical jobs.<br><br>For those who can\'t afford their augments, cheaper alternatives are available in injectibles and temporary body patches. The effects are brilliant and heralded worldwide, however there are some known side effects for many of their popular stimulants and enhancers.<br><br>Public image is a bit challenging for Medigeneix, since there are often many stories of unethical research methods, engineered treatment deficiencies, chemically addictive additives, and ruthless bill collection practices.'},

    {corpName: 'Novoline', desc: 'Paramilitary supplier specializing in physical weapons and protection.'},
    {corpName: 'Plasmigarchy', desc: 'Using plasma to provide power to the world. Holds a virtual monopoly on electricity.'},
    {corpName: 'Secubotics', desc: 'The world\'s premier manufacturer of robotic security devices or all kinds.<br><br>Automate your business security with unstoppable industrial grade protection robots of all shapes and sizes, available for all environments ranging from urban settings to jungles, or even underwater.'},
    {corpName: 'Tritoonico', desc: 'Mystical, secretive and arcane. Quasi-religious organization that teaches the fundamentals of magic power.'},
    {corpName: 'ZantoCorp', desc: 'Biggest employer of human workers. Specialized in advertising, surveillance, and information channels.'},
    {corpName: 'Unaffiliated', desc: 'Not affiliated with any MetroCorp, officially.'},
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