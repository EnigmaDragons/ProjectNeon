<script>
  let fileinput;
  export let onTextLoaded = t => {};
  export const selectFile = () => fileinput.click();
  const onFileSelected = (e) =>
  {
      var file = e.target.files[0];
      var reader = new FileReader();
      reader.onload = (r => () => onTextLoaded(r.result))(reader);
      reader.readAsText(file);
  };
</script>

<input style="display:none" type="file" accept=".json" on:change={onFileSelected} bind:this={fileinput} />
