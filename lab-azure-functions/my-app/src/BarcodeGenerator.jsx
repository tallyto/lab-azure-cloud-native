import React, { useState } from "react";

function BarcodeGenerator() {
  const [valorOriginal, setValorOriginal] = useState("");
  const [dataVencimento, setDataVencimento] = useState("");
  const [barcode, setBarcode] = useState("");
  const [imageBase64, setImageBase64] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setBarcode("");
    setImageBase64("");
    try {
      const response = await fetch("http://localhost:7071/api/barcode-generate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          valorOriginal: parseFloat(valorOriginal),
          dataVencimento,
        }),
      });
      const data = await response.json();
      setBarcode(data.barcode);
      setImageBase64(data.imageBase64);
    } catch (error) {
      alert("Erro ao gerar código de barras.");
    }
    setLoading(false);
  };

  return (
    <div
      style={{
        maxWidth: 420,
        margin: "3rem auto",
        padding: "2rem",
        background: "#fff",
        borderRadius: 16,
        boxShadow: "0 4px 24px rgba(0,0,0,0.08)",
        fontFamily: "Inter, Arial, sans-serif",
      }}
    >
      <h2 style={{ textAlign: "center", marginBottom: 24, color: "#2d3748" }}>
        Gerador de Código de Barras
      </h2>
      <form
        onSubmit={handleSubmit}
        style={{
          display: "flex",
          flexDirection: "column",
          gap: 18,
        }}
      >
        <div>
          <label
            htmlFor="valorOriginal"
            style={{ fontWeight: 500, color: "#4a5568" }}
          >
            Valor Original
          </label>
          <input
            id="valorOriginal"
            type="number"
            step="0.01"
            value={valorOriginal}
            onChange={(e) => setValorOriginal(e.target.value)}
            required
            style={{
              width: "100%",
              padding: "10px 12px",
              marginTop: 6,
              borderRadius: 8,
              border: "1px solid #cbd5e1",
              fontSize: 16,
              outline: "none",
              boxSizing: "border-box",
            }}
            placeholder="Ex: 120.50"
          />
        </div>
        <div>
          <label
            htmlFor="dataVencimento"
            style={{ fontWeight: 500, color: "#4a5568" }}
          >
            Data de Vencimento
          </label>
          <input
            id="dataVencimento"
            type="date"
            value={dataVencimento}
            onChange={(e) => setDataVencimento(e.target.value)}
            required
            style={{
              width: "100%",
              padding: "10px 12px",
              marginTop: 6,
              borderRadius: 8,
              border: "1px solid #cbd5e1",
              fontSize: 16,
              outline: "none",
              boxSizing: "border-box",
            }}
          />
        </div>
        <button
          type="submit"
          disabled={loading}
          style={{
            padding: "12px",
            borderRadius: 8,
            border: "none",
            background: "#3182ce",
            color: "#fff",
            fontWeight: 600,
            fontSize: 16,
            cursor: loading ? "not-allowed" : "pointer",
            transition: "background 0.2s",
            boxShadow: "0 2px 8px rgba(49,130,206,0.08)",
          }}
        >
          {loading ? "Gerando..." : "Gerar Código"}
        </button>
      </form>
      {barcode && (
        <div
          style={{
            marginTop: 32,
            padding: "1.5rem",
            background: "#f7fafc",
            borderRadius: 12,
            textAlign: "center",
            boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
          }}
        >
          <div
            style={{
              fontSize: 14,
              color: "#4a5568",
              marginBottom: 8,
              fontWeight: 500,
            }}
          >
            Código do Boleto:
          </div>
          <div
            style={{
              fontWeight: "bold",
              fontSize: 18,
              letterSpacing: 2,
              marginBottom: 18,
              color: "#2d3748",
              wordBreak: "break-all",
            }}
          >
            {barcode}
          </div>
          {imageBase64 && (
            <img
              src={`data:image/png;base64,${imageBase64}`}
              alt="Código de Barras"
              style={{
                maxWidth: "100%",
                background: "#fff",
                padding: 8,
                borderRadius: 8,
                boxShadow: "0 1px 4px rgba(0,0,0,0.06)",
              }}
            />
          )}
        </div>
      )}
    </div>
  );
}

export default BarcodeGenerator;