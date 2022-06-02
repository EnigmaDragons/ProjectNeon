const sql = require("mssql");

const query = async (cfg, queryStr, onRecordset) => {
  const conn = await sql.connect(cfg);
  const request = new sql.Request();    
  const rs = await request.query(queryStr);
  const result = rs.recordset;
  onRecordset(result);
  await conn.close();
  return result;
}

const queryAsync = async (cfg, queryStr) => {
  const conn = await sql.connect(cfg);
  const request = new sql.Request();    
  const rs = await request.query(queryStr);
  const result = rs.recordset;
  await conn.close();
  return result;
}

const create = (cfg) => {
  return ({
    query: (queryStr, onRecordset) => query(cfg, queryStr, onRecordset),
    queryAsync: (queryStr) => queryAsync(cfg, queryStr)
  })
}

module.exports = create;