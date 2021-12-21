<script>
	import Hero from '../Sections/Hero.svelte';
	import MainVideo from '../Sections/MainVideo.svelte';
	import Description from '../Sections/Description.svelte';
	import Screenshots from '../Sections/Screenshots.svelte';
	import CallToAction from '../Sections/CallToAction.svelte';
	import GameOverview from '../Sections/GameOverview.svelte';
	import DividerWithText from '../Elements/DividerWithText.svelte';
</script>

<Hero/>

<section class="py2">
	<Description/>
</section>

<section class="py2">
	<DividerWithText text="Screenshots"/>
	<Screenshots/>
</section>

<section class="py2">
	<CallToAction/>
</section>

<style>
</style>