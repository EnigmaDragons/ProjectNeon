const { parsed: cfg } = require('dotenv').config({ path: '../../.env' });
const createConnection = require('./sqlDb.js');
const mp0Metrics = require('./mp0metrics.js');
const kpis = require('./mp0kpis');

const sqlConfig = JSON.parse(cfg.SQL_CONFIG);
const db = createConnection(sqlConfig);
const eventTypes = mp0Metrics.eventTypes;
const version = '0.0.41';
const metrics = mp0Metrics.createByVersion(db, version);

kpis.getWinLossDetails(metrics).then(winLossDetails => { console.log({ version, winLossDetails })});
