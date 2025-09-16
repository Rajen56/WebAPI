
using Abcmoney_Transfer.Data;
using Abcmoney_Transfer.Models;
using Abcmoney_Transfer.View_model;
using Abcmoney_Transfer.Viewmodel;
using AbcmoneyTransfer.Models;
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Abcmoney_Transfer.Services
{
    public interface ITransactionServices
    {
        Task<dynamic> TransferAmount(TransactionInputVM vm, int Id);
        Task<IEnumerable<TransactionOutputVM>> GetUserTransactionsByDateRangeAsync(int Id, DateTime? startDate = null, DateTime? endDate = null);
    }
    public class TransanctionServices : ITransactionServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IForexService _forexService;

        private readonly UserManager<AppUser> _userManager;
        public TransanctionServices(ApplicationDbContext dbContext, UserManager<AppUser> userManager, IForexService forexService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _forexService = forexService;
        }
        public async Task<dynamic> TransferAmount(TransactionInputVM vm, int Id)
        {
            try
            {
                var forexRate = await _forexService.GetExchangeRateForCurrencyAsync("MYR");
                var exchangeRatePerOne = forexRate.Buy / forexRate.Unit;
                var user = await _userManager.FindByIdAsync(Id.ToString());
                var transaction = new Transactioninformation
                {
                    TransactionCreatedDate = DateTime.Now,
                    TransactionCreatedBy = Id,
                    SenderAddress = user.Address,
                    SenderCountry = user.Country,
                    SenderFirstName = user.FirstName,
                    SenderLastName = user.LastName,
                    SenderMiddleName = user.MiddleName,
                    ReceiverFirstName = vm.ReceiverFirstName,
                    ReceiverMiddleName = vm.ReceiverMiddleName,
                    ReceiverLastName = vm.ReceiverLastName,
                    ReceiverAddress = vm.ReceiverAddress,
                    ReceiverCountry = vm.ReceiverCountry,
                    BankName = vm.BankName,
                    AccountNumber = vm.AccountNumber,
                    TransferAmount = vm.TransferAmount,
                    ExchangeRate = exchangeRatePerOne,
                    PayOutAmount = vm.TransferAmount * exchangeRatePerOne
                };
                await _dbContext.Set<Transactioninformation>().AddAsync(transaction);
                var data = await _dbContext.SaveChangesAsync();
                return data;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<IEnumerable<TransactionOutputVM>> GetUserTransactionsByDateRangeAsync(int Id, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date.");
            }
            var query = _dbContext.Set<Transactioninformation>().AsQueryable();

            // Apply date filtering only if dates are provided
            if (startDate.HasValue)
            {
                query = query.Where(t => t.TransactionCreatedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(t => t.TransactionCreatedDate <= endDate.Value);
            }
            // Map to the output view model (including the required TransferAmount property)
            return await (from t in query
                          join ap in _dbContext.Set<AppUser>()
                          on t.TransactionCreatedBy equals ap.Id
                          select new TransactionOutputVM
                          {
                              TransactionId = t.TransactionId,
                              SenderAddress = t.SenderAddress,
                              SenderCountry = t.SenderCountry,
                              SenderFirstName = t.SenderFirstName,
                              SenderLastName = t.SenderLastName,
                              SenderMiddleName = t.SenderMiddleName,
                              ReceiverFirstName = t.ReceiverFirstName,
                              ReceiverMiddleName = t.ReceiverMiddleName,
                              ReceiverLastName = t.ReceiverLastName,
                              ReceiverAddress = t.ReceiverAddress,
                              ReceiverCountry = t.ReceiverCountry,
                              BankName = t.BankName,
                              AccountNumber = t.AccountNumber,
                              TransferAmount = t.TransferAmount,
                              ExchangeRate = t.ExchangeRate,
                              PayOutAmount = t.PayOutAmount,
                              TransactionCreatedDate = t.TransactionCreatedDate,
                              TransactionCreatedBy = $"{ap.FirstName} {ap.MiddleName ?? ""} {ap.LastName}"
                             
                          })
                .ToListAsync();
        }
    }
}
