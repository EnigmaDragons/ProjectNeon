<script>
	import TextFileUpload from './Elements/TextFileUpload.svelte';
	import DynamicTable from './Components/DynamicTable.svelte';
	import testData from './testData.json';

	let playReport
	let beginUpload
	let tableViewName = "cardSummary"
	let tableViews = []
	let tableData= []

	const setTableViewName = (t) => { 
		console.log(t)
		tableViewName = tableViewName = t
		tableData = playReport[tableViewName]
	}

	const setReport = (r) => {
		playReport = r
		console.log({ playReport })
		tableViews = Object.keys(playReport)
		tableData = playReport[tableViewName]
		console.log(tableData)
	}

	const onJsonImport = j => { 
		console.log('Import Begun')
		setReport(JSON.parse(j))
	};

	setReport(testData)
</script>

<TextFileUpload onTextLoaded={onJsonImport} bind:selectFile={beginUpload}/>

<main>
	<button on:click={beginUpload}>Upload</button>
	{#if playReport}
		{#each tableViews as t}
			<button on:click={() => setTableViewName(t)}>{t}</button>
		{/each}
	{/if}

	{#if !!tableViewName}
		<DynamicTable data={tableData}/>
	{/if}
</main>

<style>
	main {
		text-align: center;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>