const getVersionNumberOfPlayers = async (metrics) => {
  const uniqueInstalls = await metrics.queryUniqueInstalls();
  return uniqueInstalls.length;
}

const getWinLossDetails = async (metrics) => {
  const winLossEvents = await metrics.queryGamesWonOrLost();
  const result = { 
    wins: 0, 
    losses: 0 
  };
  winLossEvents.forEach(wl => {
    if (wl.EventType == 'gameLost') {
     ++result.losses;
    } else {
     ++result.wins;
    }
  });
  result.totalRuns = result.wins + result.losses;
  result.winRate = result.wins / result.totalRuns;
  return result;
}

module.exports = { getVersionNumberOfPlayers, getWinLossDetails };