// CSV Writer
const csvEnclosedVal = (val) => (typeof val == "string") ? '"' + val + '"' : val;
const csvRow = (vals) => vals.map(v => csvEnclosedVal(v)).join(",");
const toCsv = (data) => {
  if (!data || !data.length || data.length < 1)
    throw new Error('Invalid Argument - Cannot Convert To CSV');
  const csv = [];
  csv.push(csvRow(Object.keys(data[0])));
  for (let i = 0; i < data.length; i++)
    csv.push(csvRow(Object.values(data[i])));
  return csv.join("\r\n");
};

module.exports = toCsv;