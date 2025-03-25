const { TableClient, AzureSASCredential } = require("@azure/data-tables");
const { v4: uuidv4 } = require('uuid');

let persons = [
  //TODO could probably do something more intelligent about the partitionKey and rowKey
  { name: 'Luiz', age: 32, id: 0, partitionKey: uuidv4(), rowKey: uuidv4() },
  { name: 'Peter', age: 26, id: 1, partitionKey: uuidv4(), rowKey: uuidv4() }
];

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
  for await (const person of persons) {
    await client.createEntity(person);
  }
}

main();
