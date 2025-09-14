namespace Abcmoney_Transfer.View_model
{
    
       public class TransactionInputVM
        {
            public required string ReceiverFirstName { get; set; }
            public string? ReceiverMiddleName { get; set; }
            public required string ReceiverLastName { get; set; }
            public required string ReceiverAddress { get; set; }
            public string ReceiverCountry { get; set; } = "Nepal"; // Default receiver country
            public required string BankName { get; set; }
            public required string AccountNumber { get; set; }
            public required decimal TransferAmount { get; set; }

        }
        public class TransactionOutputVM
        {
            public int TransactionId { get; set; }
            public string SenderFirstName { get; set; }
            public string SenderMiddleName { get; set; }
            public string SenderLastName { get; set; }
            public string SenderAddress { get; set; }
            public string SenderCountry { get; set; }
            public required string ReceiverFirstName { get; set; }
            public string? ReceiverMiddleName { get; set; }
            public required string ReceiverLastName { get; set; }
            public required string ReceiverAddress { get; set; }
            public string ReceiverCountry { get; set; } = "Nepal"; // Default receiver country
            public required string BankName { get; set; }
            public required string AccountNumber { get; set; }
            public required decimal TransferAmount { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal PayOutAmount { get; set; }
            public string TransactionCreatedBy { get; set; }
            public DateTime TransactionCreatedDate { get; set; }

        }
}


