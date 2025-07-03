// promptClient.js
// Client for calling all People API endpoints

const BASE_URL = 'http://localhost:3000';
const AUTH_HEADERS = {
  username: 'calaca',
  password: '12345',
  'content-type': 'application/json',
};

async function getAllPeople() {
  const res = await fetch(`${BASE_URL}/people`, {
    method: 'GET',
    headers: AUTH_HEADERS,
  });
  return res.json();
}

async function getPersonById(id) {
  const res = await fetch(`${BASE_URL}/people/${id}`, {
    method: 'GET',
    headers: AUTH_HEADERS,
  });
  return res.json();
}

async function createPerson(name, age) {
  const res = await fetch(`${BASE_URL}/people`, {
    method: 'POST',
    headers: AUTH_HEADERS,
    body: JSON.stringify({ name, age }),
  });
  return res.json();
}

async function deletePeople() {
  const res = await fetch(`${BASE_URL}/people`, {
    method: 'DELETE',
    headers: AUTH_HEADERS,
  });
  return res.json();
}

async function searchPeople(name) {
  const res = await fetch(`${BASE_URL}/people/search?name=${encodeURIComponent(name)}`, {
    method: 'GET',
    headers: AUTH_HEADERS,
  });
  return res.json();
}

module.exports = {
  getAllPeople,
  getPersonById,
  createPerson,
  deletePeople,
  searchPeople,
};

if (require.main === module) {
  const [,, cmd, ...args] = process.argv;
  (async () => {
    try {
      switch (cmd) {
        case 'getAll':
          console.log(await getAllPeople());
          break;
        case 'getById':
          if (!args[0]) throw new Error('Usage: getById <id>');
          console.log(await getPersonById(args[0]));
          break;
        case 'create':
          if (!args[0] || !args[1]) throw new Error('Usage: create <name> <age>');
          console.log(await createPerson(args[0], Number(args[1])));
          break;
        case 'delete':
          console.log(await deletePeople());
          break;
        case 'search':
          if (!args[0]) throw new Error('Usage: search <name>');
          console.log(await searchPeople(args[0]));
          break;
        default:
          console.log('Usage: node promptClient.js <getAll|getById|create|delete|search> [args]');
      }
    } catch (e) {
      console.error(e.message);
      process.exit(1);
    }
  })();
}
