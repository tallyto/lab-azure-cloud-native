const express = require('express');
const CepController = require('../controllers/cep.controller');

const router = express.Router();
const cepController = new CepController();

router.get('/cep/:cep', cepController.getCepData.bind(cepController));

module.exports = router;