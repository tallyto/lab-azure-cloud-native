const axios = require('axios');

class CepService {
    async fetchCepData(cep) {
        console.log(`Fetching data for CEP: ${cep}`);
        const response = await axios.get(`https://viacep.com.br/ws/${cep}/json/`);
        return response.data;
    }
}

module.exports = CepService;