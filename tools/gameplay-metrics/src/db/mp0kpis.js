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
  const numCompletedRuns = result.wins + result.losses
  result.winRate = naiveRound(result.wins / numCompletedRuns, 2);
  result.numCompletedRuns = numCompletedRuns;
  return result;
}

const getPlaySummary = async (metrics) => {
  const numPlayers = await getNumberOfPlayers(metrics);
  const numRunsPlayed = await getNumberOfRunsPlayed(metrics);
  const winLossDetails = await getWinLossDetails(metrics);
  return ({ numPlayers, 
    numRunsPlayed, 
    ...winLossDetails, 
    numIncompleteRuns: numRunsPlayed - winLossDetails.numCompletedRuns });
}

const enrichedWithRates = (obj, selectNumerator, selectDenominator) => {
  Object.values(obj).forEach(v => v.rate = naiveRound(selectNumerator(v) / selectDenominator(v), 2));
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
  return sortedPicksByPopularityDescending(convertObjectToNamedArray(enrichedWithRates(obj, x => x.selected, x => x.presented)));
}

const getWinLossTable = async (metrics) => {
  const winLossEvents = await metrics.queryGamesWonOrLost();
  const winLossTable = ({});
  winLossEvents.forEach(w => winLossTable[asKey(w.RunId)] = w.EventType == 'gameWon');
  return winLossTable;
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

const getHeroWinRates = async (metrics) => {
  const winLossTable = await getWinLossTable(metrics);

  const heroesAddedEvents = await metrics.queryEventsAsync(metrics.eventTypes.heroAdded);
  const result = ({});
  heroesAddedEvents.forEach(x => {
    if (x.RunId === undefined) return;  
    const victory = winLossTable[asKey(x.RunId)]; 
    if (victory === undefined) return;

    const e = JSON.parse(x.EventData);
    const hero = e.heroName;

    const key = asKey(hero);
    if (!result[key])
      result[key] = { wins: 0, losses: 0 };
    if (victory === true)
      result[key].wins++;
    if (victory === false)
      result[key].losses++;

  });
  return convertObjectToNamedArray(enrichedWithRates(result, x => x.wins, x => x.wins + x.losses))
    .sort((a, b) => ((b.rate - a.rate) * 100) 
      + ((b.wins - a.wins) * 10) 
      + a.losses - b.losses);
}

const getHeroSummary = async (metrics) => {
  const selectionDetails = await getHeroSelectionDetails(metrics);
  const winRateDetails = await getHeroWinRates(metrics);
  const result = ({});
  selectionDetails
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
  winRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = ({ ...result[asKey(x.name)], ...x }) );
  return convertObjectToNamedArray(result)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses) }))
    .sort((a,b) => b.impact - a.impact);
}

const getCardPickWinRates = async (metrics) => {
  const winLossTable = await getWinLossTable(metrics);

  const events = await metrics.queryEventsAsync(metrics.eventTypes.rewardCardSelected);
  const result = ({});
  events.forEach(x => {
    if (x.RunId === undefined) return;
    const runKey = asKey(x.RunId);
    const victory = winLossTable[runKey]; 
    if (victory === undefined) return;

    const e = JSON.parse(x.EventData);
    const card = e.selected;

    const key = asKey(card);
    if (!result[key])
      result[key] = { 
        wins: ({}), 
        losses: ({}), 
      };
    if (victory === true)
      result[key].wins[runKey] = true;
    if (victory === false)
      result[key].losses[runKey] = true;

  });
  const summary = convertObjectToNamedArray(result).map(r => ({ ...r, wins: Object.keys(r.wins).length, losses: Object.keys(r.losses).length }));
  return enrichedWithRates(summary, x => x.wins, x => x.wins + x.losses)
    .sort((a, b) => ((b.rate - a.rate) * 100) 
      + ((b.wins - a.wins) * 10) 
      + a.losses - b.losses);
}

const average = (arr) => arr.reduce((a, b) => a + b) / arr.length;
const badDataEnemies = ['Nnz Assassin', 'NNZ Security']; 
const injuryToHpAttritionFactor = 16;

const getAttritionFactors = async (metrics) => { 
  const battleSummaries = await metrics.queryBattleSummaries();

  const enemySummaryItems = [];
  const enemyRating = ({});

  const enemyGroupSummaryItems = [];
  const enemyGroupRating = ({});

  battleSummaries.forEach(b => {
    if (b.totalEnemyPowerLevel < 20 || b.enemies.some(x => badDataEnemies.includes(x)))
      return;

    b.enemies.sort();
    const attritionFactor = (-b.attritionHpChange + (b.attritionInjuriesChange * injuryToHpAttritionFactor)) / b.totalEnemyPowerLevel;
    
    const groupKey = asKey(JSON.stringify(b.enemies));
    if (!enemyGroupRating[groupKey]) {
      enemyGroupRating[groupKey] = { powerLevel: b.totalEnemyPowerLevel, attritionItems: [] };    }
    enemyGroupRating[groupKey].attritionItems.push(attritionFactor);

    b.enemies.forEach(e => {
      const key = asKey(e);
      if (!enemyRating[key])
        enemyRating[key] = { name: e, attritionItems: [] };
      enemyRating[key].attritionItems.push(attritionFactor);
    });
  });
  Object.entries(enemyRating).forEach(x => {
    enemySummaryItems.push(({ name: x[0], numInstances: x[1].attritionItems.length, 
      attritionFactor: naiveRound(average(x[1].attritionItems), 3) }));
  });
  enemySummaryItems.sort((a, b) => b.attritionFactor - a.attritionFactor);
  Object.entries(enemyGroupRating).forEach(x => {
    if (x[1].attritionItems.length > 1)
      enemyGroupSummaryItems.push(({ group: x[0], numInstances: x[1].attritionItems.length, powerLevel: x[1].powerLevel, 
        attritionFactor: naiveRound(average(x[1].attritionItems), 3) }));
  });
  enemyGroupSummaryItems.sort((a, b) => b.attritionFactor - a.attritionFactor);

  return ({ enemies: enemySummaryItems, enemyGroups: enemyGroupSummaryItems });
}

const asKey = (str) => !str 
  ? 'undefined' 
  : str
    .replaceAll(' ', '')
    .replaceAll('\'', '')
    .replaceAll('[', '')
    .replaceAll(']', '')
    .replaceAll('"', '')
    .replaceAll('-', '')
    .replaceAll(',', '');

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

const getImpact = (pickRate, numTimesSelected, numWins, numLosses) => {
  return naiveRound((pickRate * 10) * numTimesSelected * getWinImpactFactor(numWins, numLosses), 2);
}

const getWinImpactFactor = (numWins, numLosses) => {
  if (numWins + numLosses === 0 || numWins === numLosses)
    return 1;
  
  const result = ((numWins / (numWins + numLosses)) - 0.5) * 10;
  return result;
}

const getCardSummary = async (metrics) => {
  const selectionDetails = await getCardSelectionDetails(metrics);
  const winRateDetails = await getCardPickWinRates(metrics);
  const result = ({});
  selectionDetails
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
  winRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = ({ ...result[asKey(x.name)], ...x }));
  return convertObjectToNamedArray(result)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses) }))
    .sort((a,b) => b.impact - a.impact);
}

module.exports = { getPlaySummary, getHeroSummary, getCardSummary,
  getHeroSelectionDetails, getCardSelectionDetails, getAttritionFactors, getHeroWinRates, getCardPickWinRates, };