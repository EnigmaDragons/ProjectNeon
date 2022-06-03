const naiveRound = (num, decimalPlaces = 0) => {
  var p = Math.pow(10, decimalPlaces);
  return Math.round(num * p) / p;
}

const getNumberOfPlayers = async (metrics) => {
  const uniqueInstalls = await metrics.queryUniqueInstalls();
  return uniqueInstalls.length;
}

const getNumberOfRunsPlayed = async (metrics) => {
  const items = await metrics.queryUniqueRuns();
  return items.length;
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
  result.winRate = naiveRound(result.wins / result.totalRuns, 2);
  return result;
}

const enrichedWithRates = (obj) => {
  Object.values(obj).forEach(v => v.rate = naiveRound(v.selected / v.presented, 2));
  return obj;
}

const convertObjectToNamedArray = (obj) => {
  const entries = Object.entries(obj);
  const arr = [entries.length];
  for(let i = 0; i < entries.length; i++) {
    const e = entries[i];
    e[1].name = e[0];
    arr[i] = ({ name: e[0], ...e[1] })
  }
  return arr;
}

const sortedPicksByPopularityDescending = (arr) => {
  return arr.sort((a, b) => (b.rate - a.rate) 
    - ((a.selected - b.selected) / 100) 
    - ((b.presented - a.presented) / 1000));
}

const asPickSummary = (obj) => {
  return sortedPicksByPopularityDescending(convertObjectToNamedArray(enrichedWithRates(obj)));
}

const getHeroSelectionDetails = async (metrics) => {
  const heroesPicked = await metrics.queryHeroesPicked();
  const result = ({});
  heroesPicked.forEach(h => {
    h.heroOptions.forEach(o => { 
      if (!result[o])
        result[o] = { selected: 0, presented: 0 };
      result[o].presented++;
    });
    result[h.heroName].selected++;
  });
  return asPickSummary(result);
}

const asKey = (str) => str
  .replaceAll(' ', '')
  .replaceAll('\'', '')
  .replaceAll('-', '');

const getCardSelectionDetails = async (metrics) => {
  const cardsPicked = await metrics.queryCardsPicked();
  const result = ({});
  cardsPicked.forEach(c => { 
    c.options.forEach(o => {
      const key = asKey(o);
      if (!result[key])
        result[key] = { selected: 0, presented: 0 };
      result[key].presented++;
    });
    result[asKey(c.selected)].selected++;
  });
  return asPickSummary(result);
}

module.exports = { getNumberOfPlayers, getNumberOfRunsPlayed, getWinLossDetails, getHeroSelectionDetails, getCardSelectionDetails };