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

const getWinLossDetails = async (metrics, numRunsPlayed) => {
  const winLossEvents = await metrics.queryGamesWonOrLost();
  const result = { 
    wins: 0, 
    losses: 0,
    incompletes: numRunsPlayed - winLossEvents.length
  };
  winLossEvents.forEach(wl => {
    if (wl.EventType == 'gameLost') {
     ++result.losses;
    } else {
     ++result.wins;
    }
  });
  const numCompletedRuns = result.wins + result.losses
  result.completedWinRate = naiveRound(result.wins / numCompletedRuns, 2);
  result.totalWinRate = naiveRound(result.wins / numRunsPlayed, 2);
  result.numCompletedRuns = numCompletedRuns;
  return result;
}

const getPlaySummary = async (metrics) => {
  const numPlayers = await getNumberOfPlayers(metrics);
  const numRunsPlayed = await getNumberOfRunsPlayed(metrics);
  const winLossDetails = await getWinLossDetails(metrics, numRunsPlayed);
  return ({ 
    numPlayers, 
    numRunsPlayed, 
    ...winLossDetails, 
    numIncompleteRuns: numRunsPlayed - winLossDetails.numCompletedRuns 
  });
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
    if (!result[h.heroName])
      result[h.heroName] = { selected: 0, presented: 0 };
    h.heroOptions.forEach(o => { 
      if (!result[o])
        result[o] = { selected: 0, presented: 0 };
      result[o].presented++;
    });
    result[h.heroName].selected++;
  });
  return asPickSummary(result);
}

const getHeroSummary = async (metrics) => {
  const winLossTable = await getWinLossTable(metrics);
  const winRate = Object.values(winLossTable).filter(x => x).length / Object.values(winLossTable).length;
  const selectionDetails = await getHeroSelectionDetails(metrics);
  const winRateDetails = await getHeroWinRatesFromTable(metrics, winLossTable);

  const result = ({});
  selectionDetails
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
  winRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = ({ ...result[asKey(x.name)], ...x }) );
  return convertObjectToNamedArray(result)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses, winRate) }))
    .sort((a,b) => b.impact - a.impact);
}

const getHeroWinRatesFromTable = async (metrics, winLossTable) => {
  return await getWinRatesFromTable(metrics, metrics.eventTypes.heroAdded, winLossTable, e => e.heroName);
}

const getCardPickWinRatesFromTable = async (metrics, winLossTable) => {
  return await getWinRatesFromTable(metrics, metrics.eventTypes.rewardCardSelected, winLossTable, e => e.selected);
}

const getWinRatesFromTable = async (metrics, eventType, winLossTable, selectItemKey) => {
  const events = await metrics.queryEventsAsync(eventType);
  const result = ({});
  events.forEach(x => {
    if (x.RunId === undefined) return;
    const runKey = asKey(x.RunId);
    const victory = winLossTable[runKey]; 
    if (victory === undefined) return;

    const e = JSON.parse(x.EventData);
    const key = asKey(selectItemKey(e));
    if (!result[key])
      result[key] = { wins: ({}), losses: ({}) };
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
    .replaceAll(',', '')
    .replaceAll(':', '')
    .replaceAll('\\n', '');

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

const getImpact = (pickRate, numTimesSelected, numWins, numLosses, versionWinrate) => {
  return naiveRound((pickRate * 10) * numTimesSelected * getWinImpactFactor(numWins, numLosses, versionWinrate), 2);
}

const getWinImpactFactor = (numWins, numLosses, versionWinrate) => {
  if (numWins + numLosses === 0 || numWins === numLosses)
    return 1;
  
  const result = ((numWins / (numWins + numLosses)) - versionWinrate) * 10;
  return result;
}

const getCardSummary = async (metrics) => {
  const winLossTable = await getWinLossTable(metrics);
  const winRate = Object.values(winLossTable).filter(x => x).length / Object.values(winLossTable).length;
  const selectionDetails = await getCardSelectionDetails(metrics);
  const winRateDetails = await getCardPickWinRatesFromTable(metrics, winLossTable);
  const result = ({});
  selectionDetails
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
  winRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => result[asKey(x.name)] = ({ ...result[asKey(x.name)], ...x }));
  return convertObjectToNamedArray(result)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses, winRate) }))
    .sort((a,b) => b.impact - a.impact);
}

const createLevelUpItemKey = (itemName) => asKey(`Item${itemName}`);
const createItemHeroLevelKey = (p, itemName) => asKey(`Hero${p.heroName}Level${p.level}${itemName}`);

const getLevelUpSelectionDetails = async (metrics) => {
  const picked = await metrics.queryLevelUpsPicked();
  const itemResult = ({});
  const heroLevelResult = ({});
  picked.forEach(p => {
    p.options.forEach(o => {
      const heroKey = createItemHeroLevelKey(p, o);
      if (!heroLevelResult[heroKey])
        heroLevelResult[heroKey] = { selected: 0, presented: 0 };
      heroLevelResult[heroKey].presented++;

      const key = createLevelUpItemKey(o);
      if (!itemResult[key])
        itemResult[key] = { selected: 0, presented: 0 };
      itemResult[key].presented++;
    });
    heroLevelResult[createItemHeroLevelKey(p, p.selection)].selected++;
    itemResult[createLevelUpItemKey(p.selection)].selected++;
  });
  return ({ items: asPickSummary(itemResult), heroes: asPickSummary(heroLevelResult) });
}

const getLevelUpSummary = async (metrics) => {
  const winLossTable = await getWinLossTable(metrics);
  const winRate = Object.values(winLossTable).filter(x => x).length / Object.values(winLossTable).length;
  const selectionDetails = await getLevelUpSelectionDetails(metrics);
  
  const itemWinRateDetails = await getWinRatesFromTable(metrics, metrics.eventTypes.heroLevelUp, 
    winLossTable, e => createLevelUpItemKey(e.selection));
  const itemResult = ({});
  selectionDetails.items
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => itemResult[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
  itemWinRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => itemResult[asKey(x.name)] = ({ ...itemResult[asKey(x.name)], ...x }));
  const itemSummary = convertObjectToNamedArray(itemResult)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses, winRate) }))
    .sort((a,b) => b.impact - a.impact);

  const heroItemWinRateDetails = await getWinRatesFromTable(metrics, metrics.eventTypes.heroLevelUp, 
    winLossTable, e => createItemHeroLevelKey(e, e.selection));
  const heroItemResult = ({});
  selectionDetails.heroes
    .map(x => ({ name: x.name, selected: x.selected, presented: x.presented, pickRate: x.rate }))
    .forEach(x => heroItemResult[asKey(x.name)] = { ...x, winRate: NaN, wins: 0, losses: 0,  });
    heroItemWinRateDetails
    .map(x => ({ name: x.name, wins: x.wins, losses: x.losses, winRate: x.rate }))
    .forEach(x => heroItemResult[asKey(x.name)] = ({ ...heroItemResult[asKey(x.name)], ...x }));
  const heroItemSummary = convertObjectToNamedArray(heroItemResult)
    .map(x => ({ ...x, impact: getImpact(x.pickRate, x.selected, x.wins, x.losses, winRate) }))
    .sort((a,b) => a.name.localeCompare(b.name));

  return ({ items: itemSummary, heroes: heroItemSummary });
}

// const getAdventureProgressMetrics = async (metrics) => 

module.exports = { getPlaySummary, getHeroSummary, getCardSummary,
  getHeroSelectionDetails, getCardSelectionDetails, getAttritionFactors, getLevelUpSelectionDetails, getLevelUpSummary };