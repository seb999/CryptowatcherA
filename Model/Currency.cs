using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace CryptowatcherA.Model
{
    public class Currency
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
