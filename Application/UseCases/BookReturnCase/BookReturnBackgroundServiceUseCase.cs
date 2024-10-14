using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.BookReturnCase
{
    public class BookReturnBackgroundServiceUseCase
    {
        private readonly CheckReturnDatesUseCase _checkReturnDatesUseCase;
        private readonly ILogger<BookReturnBackgroundServiceUseCase> _logger;

        public BookReturnBackgroundServiceUseCase(CheckReturnDatesUseCase checkReturnDatesUseCase, ILogger<BookReturnBackgroundServiceUseCase> logger)
        {
            _checkReturnDatesUseCase = checkReturnDatesUseCase;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _checkReturnDatesUseCase.Execute(); 
                _logger.LogInformation("Checked return dates.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}


