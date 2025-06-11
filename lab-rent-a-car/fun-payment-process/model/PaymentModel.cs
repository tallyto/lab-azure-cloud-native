using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun_payment_process.model
{
    public class PaymentModel
    {
        private string _id;
        public string id { get => _id; set { _id = value; IdPayment = value; } }
        public string IdPayment { get; set; }
        public string PartitionKey => IdPayment;
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Modelo { get; set; }
        public int Ano { get; set; }
        public string? TempoAluguel { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataInsercao { get; set; }
        public string? Status { get; set; }
        public DateTime? DataAprovacao { get; set; }

        public PaymentModel()
        {
            _id = Guid.NewGuid().ToString();
            IdPayment = _id;
            DataInsercao = DateTime.Now;
        }
    }
}
