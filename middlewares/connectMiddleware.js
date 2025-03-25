const { TableClient, AzureSASCredential } = require("@azure/data-tables");

const connectMiddleware = (req, res, next) => {
  const account = process.env.AZURE_TABLE_ACCOUNT;
  const sas = process.env.AZURE_TABLE_SAS;
  const tableName = process.env.AZURE_TABLE_TABLENAME;

  if (!account || !sas || !tableName) {
    return res.status(500).json({ error: "Missing required environment variables" });
  }
  
  const credential = new AzureSASCredential(sas);
  const client = new TableClient(
    `https://${account}.table.core.windows.net`,
    tableName,
    credential
  );
  req.tableClient = client;
  
  next();
}
  
module.exports = connectMiddleware;