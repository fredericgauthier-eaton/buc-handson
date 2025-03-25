const { v4: uuidv4 } = require('uuid');

module.exports = () => {
    const searchPeople = async (req, res, next) => {
        if (!req.query.name) return next();

        const entities = await req.tableClient.listEntities({
            queryOptions: { filter: `name eq '${req.query.name}'` }
        });
        const results = [];
        for await (const entity of entities) {
            const { age, id, name, ...rest } = entity;
            results.push({age, id, name});
        }

        return res.status(200).json(results);
    };

    const getAllPeople = async (req, res, next) => {
        const entities = await req.tableClient.listEntities();
        const results = [];
        for await (const entity of entities) {
            const { age, id, name, ...rest } = entity;
            results.push({age, id, name});
        }

        return res.status(200).json(results);
    };

    const getById = async (req, res, next) => {
        const { id } = req.params;

        const entities = await req.tableClient.listEntities({
            queryOptions: { filter: `id eq '${id}'` }
        });
        const results = [];
        for await (const entity of entities) {
            const { age, id, name, ...rest } = entity;
            results.push({age, id, name});
        }

        return res.status(200).json(results);
    };

    const createPeople = async (req, res, next) => {
        const { name, age } = req.body;

        // this is pretty bad, but it's just an example and the "legacy" code we're replacing uses a number incremental
        //  ID. The alternative would be to store the ID in anothe table, but I don't have a SAS for that. Another 
        // alternative would be to use the partition key (i.e. a UUID) but that would break compatibility.
        const entities = await req.tableClient.listEntities();
        let maxId = 0;
        for await (const entity of entities) {
            if (entity.id && !isNaN(entity.id)) {
                maxId = Math.max(maxId, parseInt(entity.id, 10));
            }
        }
        const id = maxId + 1;

        person = { id, name, age, partitionKey: uuidv4(), rowKey: uuidv4() };
        await req.tableClient.createEntity(person);

        res.status(201).json({ id, name, age });
    };

    return {
        createPeople,
        searchPeople,
        getAllPeople,
        getById
    };
};