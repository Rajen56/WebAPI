using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Abcmoney_Transfer.Models
{
    public class Transactioninformation
    {
        [Key]
        public int TransactionId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderMiddleName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCountry { get; set; } = "Malaysia";
        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverCountry { get; set; } = "Nepal"; 
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TransferAmount { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal ExchangeRate { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PayOutAmount { get; set; }
        public int TransactionCreatedBy { get; set; }
        public DateTime TransactionCreatedDate { get; set; }
    }
}
