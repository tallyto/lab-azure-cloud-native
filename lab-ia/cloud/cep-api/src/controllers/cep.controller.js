const CepService = require('../services/cep.service');

class CepController {
    async getCepData(req, res) {
        const cep = req.params.cep;
        try {
            const cepService = new CepService();
            const data = await cepService.fetchCepData(cep);
            if (!data) {
                return res.status(404).json({ message: 'CEP not found' });
            }
            res.json(data);
        } catch (error) {
            res.status(500).json({ message: 'Error fetching data', error: error.message });
        }
    }
}

module.exports = CepController;