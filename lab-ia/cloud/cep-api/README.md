# cep-api/README.md

# CEP API

This project is a Node.js API that retrieves address information based on a Brazilian postal code (CEP) using the Via Cep API.

## Features

- Receive a CEP as a parameter.
- Fetch address data from the Via Cep API.
- Return the address data in JSON format.

## Installation

1. Clone the repository:
   ```
   git clone <repository-url>
   ```

2. Navigate to the project directory:
   ```
   cd cep-api
   ```

3. Install the dependencies:
   ```
   npm install
   ```

4. Create a `.env` file in the root directory and add any necessary environment variables.

## Usage

1. Start the server:
   ```
   npm start
   ```

2. Make a GET request to the endpoint:
   ```
   GET /cep/:cep
   ```
   Replace `:cep` with the desired postal code.

## Example

To fetch data for the postal code 01001-000:
```
GET /cep/01001-000
```

## License

This project is licensed under the MIT License.