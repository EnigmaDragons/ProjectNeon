<script>    
  export let data
  import { Datatable } from 'svelte-simple-datatables'

  console.log(data)

  // https://vincjo.fr/svelte-simple-datatables/?ref=madewithsvelte.com#/settings
  const settings = { columnFilter: true }

  const dataArray = Array.isArray(data) ? data : [data]
  const columns = Object.keys(dataArray[0]).map(o => o)
  console.log({ columns })

  let rows
</script>

<section>
  <Datatable settings={settings} data={dataArray} bind:dataRows={rows}>
      <thead>
          {#each columns as c}
            <th data-key={c}><b>{c}</b></th>
          {/each}
      </thead>
      <tbody>
      {#if rows}
          {#each $rows as row}
          <tr>            
            {#each columns as c}
              <td>{row[c]}</td>
            {/each}
          </tr>
          {/each}
      {/if}
      </tbody>
  </Datatable>
</section>

<style>
section {
  height: calc(100vh - 100px);
}
</style>
