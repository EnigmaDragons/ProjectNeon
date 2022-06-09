const { parsed: cfg } = require('dotenv').config({ path: '../../.env' });
const createConnection = require('./sqlDb.js');
const mp0Metrics = require('./mp0metrics.js');
const kpis = require('./mp0kpis');
const fs = require('fs');
const toCsv = require('../csvWriter');

const sqlConfig = JSON.parse(cfg.SQL_CONFIG);
const db = createConnection(sqlConfig);
const eventTypes = mp0Metrics.eventTypes;
const version = '0.0.43';
const metrics = mp0Metrics.createByVersion(db, version);

const report = ({ version, playSummary: null, heroSummary: null, cardSummary: null });

const writeReport = () => fs.writeFileSync(`./${version}-report.json`, JSON.stringify(report, null, 2));

kpis.getPlaySummary(metrics).then(details => {
  report.playSummary = details;
  //console.log({ version, details });
  writeReport();
  //fs.writeFileSync(`./${version}-play-report.csv`, toCsv(details)); 
});
kpis.getHeroSummary(metrics).then(details => {
  report.heroSummary = details;
  //console.log({ version, details });
  writeReport();
  fs.writeFileSync(`./${version}-hero-report.csv`, toCsv(details)); 
});
kpis.getCardSummary(metrics).then(details => {
  report.cardSummary = details;
  //console.log({ version, details });
  writeReport();
  fs.writeFileSync(`./${version}-card-report.csv`, toCsv(details)); 
});
kpis.getAttritionFactors(metrics).then(details => {
  report.attritionFactors = details;
  //console.log({ version, enemies: details.enemies, enemyGroups: details.enemyGroups });
  writeReport();
  fs.writeFileSync(`./${version}-attrition-enemy-report.csv`, toCsv(details.enemies)); 
  fs.writeFileSync(`./${version}-attrition-enemy-groups-report.csv`, toCsv(details.enemyGroups)); 
});
