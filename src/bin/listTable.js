const { TableClient, AzureSASCredential } = require("@azure/data-tables");

require("dotenv").config()

const account = process.env.AZURE_TABLE_ACCOUNT;
const sas = process.env.AZURE_TABLE_SAS;
const tableName = process.env.AZURE_TABLE_TABLENAME;

const credential = new AzureSASCredential(sas);
const client = new TableClient(
  `https://${account}.table.core.windows.net`,
  tableName,
  credential
);

async function main() {
  const entities = client.listEntities();
  let i = 1;
  for await (const entity of entities) {
    console.log(`Entity ${i}: ${entity.name}`);
    i++;
  }
}

main();
