const tblName = 'MetroplexZero.GeneralMetrics';

const eventTypes = ({
  heroLevelUp: 'heroLevelUp',
  heroAdded: 'heroAdded',
  gameWon: 'gameWon',
  gameLost: 'gameLost',
  rewardCardSelected: 'rewardCardSelected',
  rewardGearSelected: 'rewardGearSelected',
  battleSummary: 'battleSummary',
  selectedSquad: 'selectedSquad',
  mapNodeSelected: 'mapNodeSelected',
});

const queryEvents = (db, version, eventType, onEvents) => {
  const q = `SELECT * FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND EventType LIKE '%${eventType}'`;
  console.log({ q });
  return db.query(q, onEvents);
}

const queryEventTypes = (db, onEventTypes) => {
  return db.query(`SELECT DISTINCT EventType FROM ${tblName}`, onEventTypes); 
}

const queryUniqueInstalls = (db, version) => {
  return db.queryAsync(`SELECT DISTINCT InstallId FROM ${tblName} WHERE GameVersion LIKE '%${version}%'`);
}

const queryUniqueRuns = (db, version) => {
  return db.queryAsync(`SELECT DISTINCT RunId FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND RunId != 'Not Initialized'`);
}

const queryGamesWonOrLost = (db, version) => {
  return db.queryAsync(`SELECT * FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND EventType IN ('gameWon', 'gameLost')`);
}

const queryHeroesPicked = (db, version) => {
  return db.queryAsyncEventData(`SELECT * FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND EventType = '${eventTypes.heroAdded}'`);
}

const queryCardsPicked = (db, version) => {  
  return db.queryAsyncEventData(`SELECT * FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND EventType = '${eventTypes.rewardCardSelected}'`);
}

const createByVersion = (db, version) => ({
  queryEvents: (eventType, onEvents) => queryEvents(db, version, eventType, onEvents),
  queryUniqueInstalls: () => queryUniqueInstalls(db, version),
  queryUniqueRuns: () => queryUniqueRuns(db, version),
  queryGamesWonOrLost: () => queryGamesWonOrLost(db, version),
  queryHeroesPicked: () => queryHeroesPicked(db, version),
  queryCardsPicked: () => queryCardsPicked(db, version),
});

module.exports = ({ eventTypes, queryEventTypes, queryEvents, queryUniqueInstalls, createByVersion });