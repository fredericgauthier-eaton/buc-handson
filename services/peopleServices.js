module.exports = () => {
    let person = [
        { id: 0, name: 'Luiz', age: 32 },
        { id: 1, name: 'Peter', age: 26 }
    ];

    const searchPeople = (req, res, next) => {
        if (!req.query.name) return next();

        const results = person.filter(person => person.name.includes(req.query.name));
        return res.status(200).json(results);
    };

    const getAllPeople = (req, res, next) => {
        console.log("getAllPeople: " + JSON.stringify(person));
        return res.status(200).json(person);
    };

    const getById = (req, res, next) => {
        const { id } = req.params;
        const result = person.find(person => person.id === parseInt(id, 10));

        if (!result) {
            return next({ status: 404, message: 'Person not found' });
        }

        return res.status(200).json(result);
    };

    const createPeople = (req, res, next) => {
        const { name, age } = req.body;
        const id = person.length;

        person.push({ id, name, age });

        res.status(201).json({ id, name, age });
    };

    return {
        createPeople,
        searchPeople,
        getAllPeople,
        getById
    };
};