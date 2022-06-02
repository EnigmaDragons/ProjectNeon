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

const queryGamesWonOrLost = (db, version) => {
  return db.queryAsync(`SELECT * FROM ${tblName} WHERE GameVersion LIKE '%${version}%' AND EventType IN ('gameWon', 'gameLost')`);
}

const createByVersion = (db, version) => ({
  queryEvents: (eventType, onEvents) => queryEvents(db, version, eventType, onEvents),
  queryUniqueInstalls: () => queryUniqueInstalls(db, version),
  queryGamesWonOrLost: () => queryGamesWonOrLost(db, version),
});

module.exports = ({ eventTypes, queryEventTypes, queryEvents, queryUniqueInstalls, createByVersion });