# 💳 Gerador de Código de Barras - Azure Functions + React

Este laboratório demonstra como criar uma solução cloud native para geração de boletos/códigos de barras utilizando **Azure Functions** (C#) e uma interface web **React**.

---

## 📦 Estrutura do Projeto

```
lab-azure-functions/
├── DIOFuncHttp/              # Azure Function HTTP Trigger (exemplo)
├── fnGeradorBoletos/         # Azure Function para geração de boletos
├── my-app/                   # Aplicação React (frontend)
└── readme.md                 # Este arquivo
```

---

## 🚀 Como Executar Localmente

### 1. Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local)
- [Node.js](https://nodejs.org/)
- [Yarn](https://yarnpkg.com/) ou npm

---

### 2. Backend - Azure Functions

```bash
# Acesse a pasta da Function
cd lab-azure-functions/fnGeradorBoletos

# Restaure dependências
dotnet restore

# Inicie a Azure Function localmente
func start
```

A função estará disponível em:  
`http://localhost:7071/api/barcode-generate`

---

### 3. Frontend - React

```bash
# Acesse a pasta do frontend
cd lab-azure-functions/my-app

# Instale as dependências
yarn install
# ou
npm install

# Inicie o app React
yarn start
# ou
npm start
```

Acesse [http://localhost:3000](http://localhost:3000) no navegador.

---

## 🖥️ Como Usar

1. Informe o **valor original** e a **data de vencimento**.
2. Clique em **Gerar Código**.
3. O código do boleto será exibido, junto com a imagem do código de barras em base64.

---

## 📑 Exemplo de Requisição/Resposta

**Requisição:**
```json
POST /api/barcode-generate
{
  "valorOriginal": 120.50,
  "dataVencimento": "2025-05-04"
}
```

**Resposta:**
```json
{
  "barcode": "0060001205020250504",
  "valorOriginal": "120.5",
  "dataVencimento": "2025-05-04",
  "imageBase64": "iVBORw0KGgoAAAANSUhEUgAAASwAAACWCAYAAABkW7XSAAAABHNCSVQICAgIfAhkiAAAAlFJREFUeJzt1EGKg0AARcHuuf+dOytBxBjDJAxvqNqp36gQ3lxrrXHTnHOMMcZaa8w5x3brdn67drZ9du3s/v3x8Xnv/u7V7my7/9ZX73vn+Dfbs2/4xvZ4/hPvbvu/tq82V/+z/f1X2zt+3loD/CHBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsIAMwQIyBAvIECwgQ7CADMECMgQLyBAsIEOwgAzBAjIEC8gQLCBDsICMB8r46Siri0LwAAAAAElFTkSuQmCC"
}
```

---

## 🛠️ Customização

- O endpoint da API pode ser alterado no arquivo [`src/BarcodeGenerator.jsx`](my-app/src/BarcodeGenerator.jsx).
- O layout pode ser customizado conforme sua necessidade.

---

## 📚 Referências

- [Documentação Azure Functions](https://learn.microsoft.com/azure/azure-functions/)
- [Documentação React](https://react.dev/)