const { parsed: cfg } = require('dotenv').config({ path: '../../.env' });
const createConnection = require('./sqlDb.js');
const mp0Metrics = require('./mp0metrics.js');
const kpis = require('./mp0kpis');
const fs = require('fs');

const sqlConfig = JSON.parse(cfg.SQL_CONFIG);
const db = createConnection(sqlConfig);
const eventTypes = mp0Metrics.eventTypes;
const version = '%';
const metrics = mp0Metrics.createByVersion(db, version);

const report = ({ version });
const time = new Date().getTime();

const writeReport = () => fs.writeFileSync(`./tempreport.json`, JSON.stringify(report, null, 2));

kpis.getWinLossDetails(metrics).then(winLossDetails => { 
  report.winLoss = winLossDetails;
  console.log({ version, winLossDetails });
  writeReport();
});
kpis.getHeroSelectionDetails(metrics).then(details => { 
  report.heroSelections = details;
  console.log({ version, details });
  writeReport();
});
kpis.getNumberOfRunsPlayed(metrics).then(n => { 
  report.runsPlayed = n;
  console.log({ version, runsPlayed: n });
  writeReport();
});
kpis.getCardSelectionDetails(metrics).then(details => {
  report.cardSelections = details;
  console.log({ version, details });
  writeReport();
});

