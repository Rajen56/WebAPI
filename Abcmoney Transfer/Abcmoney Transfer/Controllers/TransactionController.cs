using ABCExchange.Services;
using ABCExchange.ViewModel;
using Abcmoney_Transfer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Abcmoney_Transfer.Controllers
{
    [Route("api/")]
    [ApiExplorerSettings(GroupName = "access-control")]
    public class TransanctionApiController : BaseApiController

    {
        private readonly IForexService _forexervice;
        private readonly ITransactionServices _transactionServices;
        public TransanctionApiController(IForexService forexervice, ITransactionServices transactionServices)
        {
            _forexervice = forexervice;
            _transactionServices = transactionServices;
        }
        [HttpGet("view/exchangeRates")]
        public async Task<ResponseModel> GetExchangeRates()
        {
            try
            {
                var data = await _forexervice.GetExchangeRatesAsync();
                if (data == null || !data.Any())
                {
                    return new ResponseModel(404, "No valid exchange rates found.");
                }

                return new ResponseModel(200, "", data.ToArray());
            }
            catch (Exception ex)
            {
                return new ResponseModel(400, $"Error: {ex.Message}");
            }
        }
        [HttpPost("transferAmount/byUser")]
        public async Task<ResponseModel> TransferAmount(TransactionInputVM vm)
        {
            try
            {
                var roles = User.Identity.GetRoles();

                if (roles.Contains("User"))
                {
                    var userId = User.Identity.GetIdentityUserId();
                    var data = await _transactionServices.TransferAmount(vm, userId);
                    if (data == null || data == 0)
                    {
                        return new ResponseModel(400, "Transaction unsuccessful!!");
                    }

                    return new ResponseModel(200, "Amount Transfered Succefully", data);
                }
                else
                {
                    return new ResponseModel(400, "Unauthorized roles", roles);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel(400, $"Error: {ex.Message}");
            }

        }

        [HttpGet("userTransactionReport")]
        public async Task<ResponseModel> TransferReportForUser(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var roles = User.Identity.GetRoles();

                if (roles.Contains("User"))
                {
                    var userId = User.Identity.GetIdentityUserId();
                    var data = await _transactionServices.GetUserTransactionsByDateRangeAsync(userId, startDate, endDate);
                    if (data.Count() <= 0)
                    {
                        return new ResponseModel(400, "No Transaction found!", null);
                    }

                    return new ResponseModel(200, "success", data);
                }
                else
                {
                    return new ResponseModel(400, "Unauthorized access", roles);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel(400, $"Error: {ex.Message}");
            }

        }

    }
}