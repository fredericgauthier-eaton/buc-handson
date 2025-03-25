const express = require('express');
const middleware = require('../middlewares/index')
const services = require('../services/peopleServices')

const peopleRouter = express.Router();

peopleRouter.post('/', middleware.authMiddleware, middleware.connectMiddleware, services.createPeople)
peopleRouter.get('/search', middleware.authMiddleware, middleware.connectMiddleware, services.searchPeople)
peopleRouter.get('/', middleware.authMiddleware, middleware.connectMiddleware, services.getAllPeople)
peopleRouter.get('/:id', middleware.authMiddleware, middleware.connectMiddleware, services.getById)

module.exports = { peopleRouter }