const express = require('express');
const cors = require('cors');
const { ServiceBusClient } = require('@azure/service-bus');

require('dotenv').config();

const app = express();

app.use(cors());
app.use(express.json());

app.post('/api/locacao', async (req, res) => {
    const { nome, email, modelo, ano, tempoAluguel, data } = req.body;
    if (!nome || !email || !modelo || !ano || !tempoAluguel || !data) {
        return res.status(400).json({ error: 'nome, email, modelo, ano, tempoAluguel e data são obrigatórios.' });
    }

    const connectionString = process.env.SB_CONN_STRING;
    const queueName = process.env.SB_QUEUE_NAME;

    if (!connectionString) {
        return res.status(500).json({ error: 'A variável de ambiente SB_CONN_STRING não está definida.' });
    }
    if (!queueName) {
        return res.status(500).json({ error: 'A variável de ambiente SB_QUEUE_NAME não está definida.' });
    }
    if (connectionString.includes('EntityPath')) {
        return res.status(500).json({ error: 'A connection string contém EntityPath. Use a connection string do namespace do Service Bus, não da fila específica.' });
    }

    const mensagem = {
        nome,
        email,
        modelo,
        ano,
        tempoAluguel,
        data,
        dataInsercao: new Date().toISOString()
    };

    let sbCliente;
    let sender;
    try {
        sbCliente = new ServiceBusClient(connectionString);
        sender = sbCliente.createSender(queueName);
        const message = {
            body: mensagem,
            contentType: 'application/json',
            subject: 'locacao',
        };
        await sender.sendMessages(message);
        res.status(200).json({ message: 'Locação registrada com sucesso!' });
    } catch (error) {
        console.error('Erro ao enviar mensagem para o Service Bus:', error);
        res.status(500).json({ error: 'Erro ao processar a locação.' });
    } finally {
        if (sender) await sender.close();
        if (sbCliente) await sbCliente.close();
    }
});

app.listen(3000, () => {
    console.log('Servidor rodando na porta 3000');
}
);