const express = require('express');
const bodyParser = require('body-parser');
const rescue = require('express-rescue');
const cors = require('cors');
const middleware = require('../middlewares');
const limiter = require('../utils/rateLimit');
const helmet = require("helmet");
const morgan = require("morgan");
const peopleServicesFactory = require('../services/peopleServices'); // Import the factory function

module.exports = () => {
    const app = express();
    const peopleServices = peopleServicesFactory(); // Create a new instance of peopleServices

    app.use(morgan("common"));
    app.use(helmet()); // for security Express.js purposes
    app.use(cors());
    app.use(limiter);
    app.use(bodyParser.json());
    app.use(middleware.authMiddleware);
    app.use(middleware.connectMiddleware);

    app.get('/people', rescue(peopleServices.getAllPeople));
    app.post('/people', rescue(peopleServices.createPeople));
    app.get('/people/:id', rescue(peopleServices.getById));

    app.use(middleware.errorMiddleware);

    return app;
};
