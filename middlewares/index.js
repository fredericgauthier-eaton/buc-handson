const authMiddleware = require('./authMiddleware')
const connectMiddleware = require('./connectMiddleware')
const errorMiddleware = require('./errorMiddleware')

module.exports = {
    authMiddleware,
    connectMiddleware,
    errorMiddleware,
}