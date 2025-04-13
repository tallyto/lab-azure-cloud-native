import streamlit as st
from azure.storage.blob import BlobServiceClient
from sqlalchemy import create_engine, text
import os
import uuid
from dotenv import load_dotenv
from sqlalchemy.engine import URL
from sqlalchemy import create_engine

# Carrega variáveis do .env
load_dotenv()

# Azure Blob Storage
blobConnectionString = os.getenv("BLOB_CONNECTION_STRING")
blobContainerName = os.getenv("BLOB_CONTAINER_NAME")
blobAccountName = os.getenv("BLOB_ACCOUNT_NAME")

# SQL Server na Azure
sqlServer = os.getenv("SQL_SERVER")
sqlDatabase = os.getenv("SQL_DATABASE")
sqlUsername = os.getenv("SQL_USERNAME")
sqlPassword = os.getenv("SQL_PASSWORD")

# Conexão com SQL Server usando SQLAlchemy + pymssql
def get_engine():
    connection_url = URL.create(
        "mssql+pymssql",
        username=sqlUsername,
        password=sqlPassword,
        host=sqlServer,
        port=1433,
        database=sqlDatabase
    )
    return create_engine(connection_url, connect_args={"autocommit": True})

# Upload da imagem no Azure Blob
def upload_image_to_blob(image_file):
    blob_service_client = BlobServiceClient.from_connection_string(blobConnectionString)
    blob_client = blob_service_client.get_blob_client(container=blobContainerName, blob=str(uuid.uuid4()) + "_" + image_file.name)
    blob_client.upload_blob(image_file, overwrite=True)
    return f"https://{blobAccountName}.blob.core.windows.net/{blobContainerName}/{blob_client.blob_name}"

# Criação da tabela (caso não exista)
def create_table():
    with get_engine().connect() as conn:
        conn.execute(text("""
            IF NOT EXISTS (
                SELECT * FROM sysobjects WHERE name='Produtos' AND xtype='U'
            )
            CREATE TABLE Produtos (
                id UNIQUEIDENTIFIER PRIMARY KEY,
                nome NVARCHAR(255),
                preco DECIMAL(10, 2),
                descricao NVARCHAR(MAX),
                imagem_url NVARCHAR(MAX)
            )
        """))

# Inserção do produto
def insert_product(nome, preco, descricao, imagem_url):
    with get_engine().connect() as conn:
        conn.execute(text("""
            INSERT INTO Produtos (id, nome, preco, descricao, imagem_url)
            VALUES (:id, :nome, :preco, :descricao, :imagem_url)
        """), {
            "id": str(uuid.uuid4()),
            "nome": nome,
            "preco": preco,
            "descricao": descricao,
            "imagem_url": imagem_url
        })

# Listagem dos produtos
def list_products():
    with get_engine().connect() as conn:
        result = conn.execute(text("SELECT nome, preco, descricao, imagem_url FROM Produtos"))
        return result.fetchall()

# Interface Streamlit
st.title("Cadastro de Produtos")

product_name = st.text_input("Nome do Produto")
product_price = st.number_input("Preço do Produto", min_value=0.0, format="%.2f")
product_description = st.text_area("Descrição do Produto")
product_image = st.file_uploader("Imagem do Produto", type=["jpg", "png", "jpeg"])

if st.button("Cadastrar produto"):
    if product_name and product_image:
        try:
            create_table()
            image_url = upload_image_to_blob(product_image)
            insert_product(product_name, product_price, product_description, image_url)
            st.success("✅ Produto cadastrado com sucesso!")
        except Exception as e:
            st.error(f"❌ Erro ao cadastrar produto: {e}")
    else:
        st.warning("Por favor, preencha o nome e selecione uma imagem.")

st.header("📦 Produtos cadastrados")

if st.button("Atualizar lista de produtos"):
    try:
        products = list_products()
        for nome, preco, descricao, imagem_url in products:
            st.subheader(nome)
            st.image(imagem_url, width=150)
            st.write(f"💲 **Preço:** R$ {preco:.2f}")
            st.write(f"📝 **Descrição:** {descricao}")
    except Exception as e:
        st.error(f"❌ Erro ao listar produtos: {e}")

if st.button("Criar Tabela (Teste)"):
    try:
        create_table()
        st.success("✅ Tabela 'Produtos' criada (ou já existia).")
    except Exception as e:
        st.error(f"❌ Erro ao criar a tabela: {e}")